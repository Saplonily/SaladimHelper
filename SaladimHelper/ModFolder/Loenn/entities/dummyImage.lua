local e = {}

e.name = "SaladimHelper/DummyImage"
e.depth = 0
e.placements = {
    name = "normal",
    data = {
        texture = "objects/refill/idle00",
        width = 8,
        height = 8
    }
}

function e.texture(room, entity, node)
    return entity.texture
end

return e
