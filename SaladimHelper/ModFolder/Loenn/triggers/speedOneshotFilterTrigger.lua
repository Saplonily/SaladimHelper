local t = {}
t.name = "SaladimHelper/SpeedOneshotFilterTrigger"
t.placements = {
    name = "normal",
    data = {
        width = 16,
        height = 16,
        strength_from = 0.0,
        strength_to = 100.0,
        speed_threshold = 500.0,
        duration = 2.0,
        effect_path = "SaladimHelper/Blur",
        index = 0.0,
        easing = "SineIn"
    }
}

t.fieldInformation = {
    easing = {
        options = require("mods").requireFromPlugin("libraries.easing_enums"),
        editable = false
    },
    effect_path = {
        options = require("mods").requireFromPlugin("libraries.filters"),
        editable = true
    }
}

return t