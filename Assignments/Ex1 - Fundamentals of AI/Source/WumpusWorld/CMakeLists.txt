cmake_minimum_required(VERSION 3.28)
find_package(Java 10 REQUIRED)
include(UseJava)

project(WumpusWorld)
file(GLOB BEHAVIOR_FILES behaviors/*.java)
set(SOURCE_FILES ../ai_structures/TreeNode.java Behavior.java Agent.java Game.java World.java)

add_jar(WumpusWorld ${SOURCE_FILES} ${BEHAVIOR_FILES} ENTRY_POINT wumpusworld.Game)
