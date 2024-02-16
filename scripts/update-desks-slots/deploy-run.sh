#!/bin/bash

# Define variables
projectName=UpdateDesksSlots
output=./deployments/$projectName

# Create output folder structure in case not exists
mkdir -p -v $output

# Deploy self contained solution on for linux64 runtime
echo "Publishing project $projectName..."
dotnet publish $projectName.csproj -r linux-x64 -o $output --sc true

# Move to output folder and run the script
cd $output || exit
echo "Running project $projectName..."
./$projectName

# Removing traces
echo "Removing deployments..."
rm -r $output

# Closing script
echo "Automated deploy and run pipeline terminated."
