-- copied from loenn/entities/key.lua

local coin = {}

coin.name = "SaladimHelper/CollectableCoin"
coin.nodeLineRenderType = "line"
coin.texture = "SaladimHelper/entities/collectableCoin/idle00"

coin.placements = {
    {
        name = "normal"
    },
    {
        name = "with_return",
        placementType = "point",
        data = {
            nodes = {
                { x = 0, y = 0 },
                { x = 0, y = 0 }
            }
        }
    }
}

function coin.nodeLimits(room, entity)
    local nodes = entity.nodes or {}
    if #nodes > 0 then
        return 2, 2
    else
        return 0, 0
    end
end

return coin