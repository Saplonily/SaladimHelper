﻿local t = {}
t.name = "SaladimHelper/FadeFilterTrigger"
t.placements = {
    name = "normal",
    data = {
        width = 16,
        height = 16,
        strength_from = 0.0,
        strength_to = 100.0,
        left_to_right = true,
        easing = "Linear",
        effect_path = "SaladimHelper/Blur",
        index = 0.0
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