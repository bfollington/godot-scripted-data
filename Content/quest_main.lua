-- Content/quest_main.lua
debug("Starting quest creation...")

-- Create the quest
local quest = createQuest("main_quest")
debug("Quest created with ID: main_quest")

-- Set properties (using dot notation for properties)
quest.Title = "Save the Kingdom"
quest.MinLevel = 10
debug("Basic properties set")

-- Add objectives using the method
quest:AddObjective("Find the ancient sword")
quest:AddObjective("Defeat the dragon")
debug("Objectives added")

-- Create and add a reward
local reward = createReward("item", "legendary_sword", 1)
quest:AddReward(reward)
debug("Reward added")

debug("Quest creation complete")
return quest
