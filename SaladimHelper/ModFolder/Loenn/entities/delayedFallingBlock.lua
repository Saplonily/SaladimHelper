local fakeTilesHelper = require("helpers.fake_tiles")

local fallingBlock = {}

fallingBlock.name = "SaladimHelper/DelayedFallingBlock"
fallingBlock.placements = {
    name = "normal",
    data = {
        tiletype = "3",
        climbFall = true,
        behind = false,
        width = 8,
        height = 8,
        preDelay = 0.2,
        playerWaitDelay = 0.4,
        noSfx = false,
        autoFall = false
    }
}

fallingBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
fallingBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

function fallingBlock.depth(room, entity)
    return entity.behind and 5000 or 0
end

return fallingBlock