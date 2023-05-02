local utils = require("utils")

local reelCamera = {}
reelCamera.name = "SaladimHelper/ReelCamera"
reelCamera.depth = -1
reelCamera.nodeLimits = {2, 200}
reelCamera.nodeVisibility = "selected"
reelCamera.nodeLineRenderType = "line"
reelCamera.placements = {
    name = "reelCamera_normal",
    data = {
        width = 80,
        height = 80,
        move_time_sequence = "1,1,4,5,1,4",
        delay_sequence = "19,1,9,8,1,0",
        start_delay = 1.0,
        start_move_time = 1.0,
        squash_horizontal_area = true,
        set_offset_on_finished = false,
        offset_x = 0,
        offset_y = 0
    }
}

function reelCamera.draw(room, entity, viewport)
    love.graphics.setColor(124 / 255, 251 / 255, 171 / 255, 0.4)
    love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    love.graphics.setColor(255, 255, 255, 255)
end

function reelCamera.nodeDraw(room, entity, node, nodeIndex, viewport)
    love.graphics.setColor(124 / 255, 251 / 255, 171 / 255, 0.7)
    love.graphics.circle("fill", node.x + entity.width / 2, node.y + entity.height / 2, 5)
    love.graphics.print(nodeIndex .. "", node.x + entity.width / 2, node.y + entity.height / 2)
    love.graphics.setColor(255, 255, 255, 255)
end

function reelCamera.nodeRectangle(room, entity, node, nodeIndex, viewport)
    return utils.rectangle(node.x + entity.width / 2 - 160, node.y + entity.height / 2 - 184 / 2, 320, 184)
end

function reelCamera.nodeOffset(room, entity)
    return {entity.x - entity.width, entity.y - entity.height}
end

return reelCamera
