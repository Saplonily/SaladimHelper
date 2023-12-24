local utils = require("utils")

local shop = {}

shop.name = "SaladimHelper/MaybeAShop"
shop.depth = -100
shop.nodeLimits = { 1, 50 }
shop.nodeLineRenderType = "fan"
shop.placements = {
    name = "normal",
    data = {
        width = 16,
        height = 16,
        cost_sequence = "1,2,3",
        tex_sequence = "x,areas/new,areas/city",
        line_max = 4
    }
}

shop.fieldInformation = {
    line_max = {
        fieldType = "integer",
        minumumValue = 1
    }
}

function shop.nodeDraw(room, entity, node, nodeIndex, viewport)
    love.graphics.setColor(124 / 255, 251 / 255, 171 / 255, 0.4)
    love.graphics.rectangle("fill", node.x - 4, node.y - 4, 8, 8)
    love.graphics.setColor(0, 0, 0, 1)
    love.graphics.print(tostring(nodeIndex), node.x - 1, node.y - 3)
    love.graphics.setColor(1, 1, 1, 1)
end

function shop.nodeLineRenderOffset(entity, node, nodeIndex)
    return 0, 0
end

function shop.nodeRectangle(room, entity, node, nodeIndex, viewport)
    return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return shop