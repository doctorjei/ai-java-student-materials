#!/usr/bin/env sh

# Find the Java runtime executable
JAVA=`env which java`

# Paths to JAR files, etc.
PLANNER_JAR=PathPlannerCLI.jar
SEARCH_JAR=./lib/uwu/student.jar
TILE_JAR=./lib/uwu/tiles.jar
DRIVER=edu.uwu.pathplanner.Console

# Run the project!
$JAVA -cp $TILE_JAR:$SEARCH_JAR:$PLANNER_JAR $DRIVER "$@"

