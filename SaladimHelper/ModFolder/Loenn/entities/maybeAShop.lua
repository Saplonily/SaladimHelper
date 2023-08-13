local shop = {}

shop.name = "SaladimHelper/MaybeAShop"
shop.depth = -100
shop.nodeLimits = { 0, 50 }
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

return shop