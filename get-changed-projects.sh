#!/bin/bash
set -e

# Get the list of changed files
changed_files=$(git diff --name-only $GITHUB_SHA $GITHUB_SHA~1)

# Initialize an array to store the projects that need to be built
projects_to_build=()

# Function to find projects that depend on a given project
find_dependent_projects() {
  local project_file=$1
  local project_dir=$(dirname $project_file)
  
  # Add the project itself to the list
  projects_to_build+=($project_dir)
  
  # Find all projects that reference this project
  grep -rl "<ProjectReference Include=\"$project_dir" . | while read dependent_project_file; do
    local dependent_project_dir=$(dirname $dependent_project_file)
    if [[ ! " ${projects_to_build[@]} " =~ " ${dependent_project_dir} " ]]; then
      find_dependent_projects $dependent_project_file
    fi
  done
}

# Iterate over changed files and find the projects to build
for file in $changed_files; do
  if [[ $file == *.csproj ]]; then
    find_dependent_projects $file
  fi
done

# Remove duplicates
projects_to_build=($(echo "${projects_to_build[@]}" | tr ' ' '\n' | sort -u | tr '\n' ' '))

# Output the projects to build as a comma-separated list
echo $(IFS=,; echo "${projects_to_build[*]}")
