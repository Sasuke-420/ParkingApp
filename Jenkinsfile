def customImage = ''
def minVerVersion=''
def dockerVersion = ''
def registryCredential = 'svc-jenkins'
def dockerTagPrefix = 'lisec/service/'
def shortCommit = ''
def latestMinorTag = ''
def repoChoices = ['NONE', 'exp.nexus.lisec.internal', 'staging.nexus.lisec.internal']
if ( env.BRANCH_NAME == "master" || env.BRANCH_NAME == "main" || env.BRANCH_NAME.startsWith("release-") )
    repoChoices = ['staging.nexus.lisec.internal', 'exp.nexus.lisec.internal', 'NONE']

pipeline {

    environment {
        DOCKER_BUILDKIT=1
    }

    agent {
        node {
            label 'docker'
        }
    }

    parameters {
        choice(name: 'REPO_CHOICE', choices: repoChoices, description: 'Choose a repo to deploy to. Choose NONE to skip deployment')
        booleanParam(name: 'DEPLOY_LATEST_MINOR_TAG', defaultValue: false, description: 'Tag and deploy as latest minor version?')
        booleanParam(name: 'KEEP_LOCAL_DOCKER_IMAGE', defaultValue: false, description: 'Do not clean away the local docker image on the build node?')
        booleanParam(name: 'SKIP_CLEANUP', defaultValue: false, description: 'Do not clean away the local build data on the build node?')
    }

    stages {
        stage('Initialize') {
            steps {
                script {
                    load 'Jenkins-env.groovy'

                    env.packagename = env.packagenameCamelCase.replaceAll( /([A-Z])/, /_$1/ ).toLowerCase().replaceAll( /^_/, '' )
                    minVerVersion = sh(returnStdout: true, script: "docker run --rm -v=${WORKSPACE}:/repo vnd.nexus.lisec.internal/lisec/minver:latest -t v -v e -b build.${env.BUILD_ID}").trim()
                    dockerVersion = minVerVersion.replaceAll( /\+/, /\./ )

                    if (params.DEPLOY_LATEST_MINOR_TAG) {
                        def versionParts = minVerVersion.tokenize('.')
                        assert (versionParts.size() >= 2 && versionParts[0].isInteger() && versionParts[1].isInteger()), "Wrong version format - expected MAJOR.MINOR.PATCH[...] - got ${minVerVersion}"
                        latestMinorTag = versionParts[0] + '.' + versionParts[1]
                    }

                    shortCommit = sh(returnStdout: true, script: "git log -n 1 --pretty=format:'%h'").trim()
                }

                writeFile file: 'version.txt', text: "${minVerVersion} g${shortCommit}"
                writeFile file: 'startup.sh', text: """#!/bin/bash
if [ \"\$1\" == \"--version\" ]
then
    cat version.txt
    echo \"\"
else
    ./${env.packagenameCamelCase}.Service \"\$@\"
fi
"""
                sh "chmod a+x startup.sh"
            }
        }
        stage('Run Tests') {
            steps {
                sh "docker build --target testresult --build-arg minVerVersion=${minVerVersion} --progress=plain --output type=local,dest=./buildoutput/ ."

                script {
                    dir("buildoutput") {
                        commonPipeline.ingestDotNetTestResults('**/*unit_tests.xml', '**/coverage.cobertura.xml')
                    }
                }
            }
        }
        stage('Run Dependency Scan') {
            steps {
                dir("buildoutput") {
                    script {
                        commonPipeline.performDependencyCheck()
                    }
                }
            }
        }
        stage('Build Final Image') {
            steps{
                script {
                    customImage = docker.build("${dockerTagPrefix}${packagename}:${dockerVersion}", "--build-arg minVerVersion=${minVerVersion} --progress=plain --target runtime .")
                }
            }
        }
        stage('Deploy Docker Image') {
            when {
                expression { params.REPO_CHOICE != 'NONE' }
            }
            steps{
                script {
                    docker.withRegistry( 'https://' + params.REPO_CHOICE, registryCredential ) {
                        customImage.push()

                        currentBuild.description = "Deployed ${params.REPO_CHOICE}/${dockerTagPrefix}${packagename}:${dockerVersion}"
                        if (params.REPO_CHOICE == 'staging.nexus.lisec.internal') {
                            currentBuild.description += " -> <a href=\"/job/NG/job/Infrastructure/job/Docker%20Publish%20Image%20to%20External%20Repository/parambuild/?DOCKER_IMAGE_AND_TAG=${dockerTagPrefix}${packagename}:${dockerVersion}\">Deploy to external repo</a>"
                        }

                        if (params.DEPLOY_LATEST_MINOR_TAG) {
                            customImage.push(latestMinorTag)
                            currentBuild.description += "<br>Also deployed latest minor: ${params.REPO_CHOICE}/${dockerTagPrefix}${packagename}:${latestMinorTag}"
                            if (params.REPO_CHOICE == 'staging.nexus.lisec.internal') {
                                currentBuild.description += " -> <a href=\"/job/NG/job/Infrastructure/job/Docker%20Publish%20Image%20to%20External%20Repository/parambuild/?DOCKER_IMAGE_AND_TAG=${dockerTagPrefix}${packagename}:${latestMinorTag}\">Deploy to external repo</a>"
                            }
                        }
                    }
                }
            }
        }
    }

    post {
        success {
            script { commonPipeline.postSuccess() }
        }
        fixed {
            script { commonPipeline.postFixed() }
        }
        unstable {
            script { commonPipeline.postUnstable() }
        }
        failure {
            script { commonPipeline.postFailure() }
        }
        always {
            warnError(message: 'Cleanup failed') {
                script {
                    if (!params.SKIP_CLEANUP) {
                        sh 'rm -Rf buildoutput/'
                    }
                    if (!params.KEEP_LOCAL_DOCKER_IMAGE) {
                        sh ("docker image rm ${dockerTagPrefix}${packagename}:${dockerVersion}")
                    }
                    if (params.REPO_CHOICE != 'NONE') {
                        sh "docker image rm ${params.REPO_CHOICE}/${dockerTagPrefix}${packagename}:${dockerVersion}"
                        if (params.DEPLOY_LATEST_MINOR_TAG) {
                            sh "docker image rm ${params.REPO_CHOICE}/${dockerTagPrefix}${packagename}:${latestMinorTag}"
                        }
                    }
                    commonPipeline.cleanDockerBuildCache()
                }
            }
        }
    }
}