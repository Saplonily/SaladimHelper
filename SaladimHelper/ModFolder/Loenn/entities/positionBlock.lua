local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local block = {}

block.name = "SaladimHelper/PositionBlock"
block.nodeLimits = { 1, 1 }
block.nodeLineRenderType = "line"
block.placements = {
    name = "normal",
    data = {
        tiletype = "3",
        width = 8,
        height = 8,
        speed = 128,
        easing = "Linear",
        range = 0
    }
}

block.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
block.nodeSprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
block.fieldInformation = {
    easing = {
        options = require("mods").requireFromPlugin("libraries.easing_enums"),
        editable = false
    },
    tiletype = function()
        return {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        }
    end
}

function block.nodeRectangle(room, entity, node)
    return utils.rectangle(node.x or 0, node.y or 0, entity.width or 8, entity.height or 8)
end

return block