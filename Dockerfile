FROM vnd.nexus.lisec.internal/lisec/dotnet/sdk:6.0 AS buildenv
ARG minVerVersion
ENV MinVerVersionOverride=$minVerVersion
COPY . /code/
WORKDIR /code/
RUN dotnet publish *.Service -c Release --use-current-runtime -o output

FROM buildenv AS testruntime
WORKDIR /code/
RUN dotnet test --logger "trx;LogFileName=unit_tests.xml" --collect:"XPlat Code Coverage" --settings coverlet.runsettings -o testout || echo "There were failing tests!"

FROM scratch as testresult
COPY --from=testruntime /code ./

FROM vnd.nexus.lisec.internal/lisec/dotnet/aspnet:6.0 AS runtime
LABEL image=runtime
WORKDIR /app
COPY --from=buildenv /code/output .
COPY --from=buildenv /code/version.txt .
COPY --from=buildenv /code/startup.sh .
RUN ls -la
ENTRYPOINT ["./startup.sh"]