local entity = {}

entity.name = "SaladimHelper/BitsMagicLantern"
entity.placements =
{
    name = "normal",
    data = {
        radius = 20.0,
        holdable = false
    }
}

function entity.texture(room, entity)
    local holdable = entity.holdable or false

    return holdable and "SaladimHelper/entities/bitsMagicLantern/holdable" or "SaladimHelper/entities/bitsMagicLantern/static0"
end

return entity