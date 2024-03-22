local fakeTilesHelper = require("helpers.fake_tiles")

local block = {}

block.name = "SaladimHelper/PositionBlock"
block.placements = {
    name = "normal",
    data = {
        tiletype = "3",
        width = 8,
        height = 8,
        leftToRight = true,
        range = -16,
        speed = 128,
        easing = "Linear"
    }
}

block.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
block.fieldInformation = {
    easing = {
        options = require("mods").requireFromPlugin("libraries.easing_enums"),
        editable = false
    },
    tiletype = {
        options = fakeTilesHelper.getTilesOptions(),
        editable = false
    }
}

return block