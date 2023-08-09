local shop = {}

shop.name = "SaladimHelper/MaybeAShop"
shop.depth = -100
shop.nodeLimits = { 0, 50 }
shop.nodeLineRenderType = "fan"
shop.placements = {
    name = "normal",
    data = {
        oneUse = false,
        width = 16,
        height = 16,
        config_path = "path/to/your/yaml"
    }
}

return shop