#!/usr/bin/env sh

# Find the Java runtime executable
JAVA=`env which java`

# Paths to JAR files, etc.
PLANNER_JAR=PathPlannerCLI.jar
DRIVER=edu.uwu.pathplanner.Console

# Run the project!
$JAVA -cp $PLANNER_JAR $DRIVER "$@"

