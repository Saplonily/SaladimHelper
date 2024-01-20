local depths = require("consts/object_depths")
local e = {}

e.name = "SaladimHelper/DummyImage"
e.depth = 0
e.placements = {
    name = "normal",
    data = {
        texture = "objects/refill/idle00",
        width = 8,
        height = 8,
        offset_x = 0,
        offset_y = 0,
        depth = 0
    }
}

function e.texture(room, entity, node)
    return entity.texture
end

function e.offset(room, entity)
    -- magic offsets
    return { -entity.offset_x - 3, -entity.offset_y - 3 }
end

e.fieldInformation = 
{
    offset_x = {
        fieldType = "integer"
    },
    offset_y = {
        fieldType = "integer"
    },
    depth = {
        options = depths,
        editable = true
    }
}

return e
