#!/bin/bash
set -e

# Get the list of projects to build
projects_to_build=$(./scripts/get-changed-projects.sh)

# Initialize an array to store the projects that need to be deployed
projects_to_deploy=()

# Check each project to see if it is packable
for project in ${projects_to_build//,/ }; do
  if grep -q '<IsPackable>true</IsPackable>' $project/*.csproj; then
    projects_to_deploy+=($project)
  fi
done

# Output the projects to deploy as a comma-separated list
echo $(IFS=,; echo "${projects_to_deploy[*]}")
