local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")

local dirtBounceBlock = {}

dirtBounceBlock.name = "SaladimHelper/DirtBounceBlock"
dirtBounceBlock.depth = 8990
dirtBounceBlock.minimumSize = {16, 16}
dirtBounceBlock.placements = {
    name = "dirtBounceBlock_normal",
    data = {
        width = 16,
        height = 16,
        none_core_mode = false
    }
}

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

local dirtBounceBlockTexture = "SaladimHelper/Entities/more_bounce_block/rock_tiles"
local dirtCrystalTexture = "SaladimHelper/Entities/more_bounce_block/rock_center00"

function dirtBounceBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local ninePatch = drawableNinePatch.fromTexture(dirtBounceBlockTexture, ninePatchOptions, x, y, width, height)
    local crystalSprite = drawableSprite.fromTexture(dirtCrystalTexture, entity)
    local sprites = ninePatch:getDrawableSprite()

    crystalSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, crystalSprite)

    return sprites
end

return dirtBounceBlock
