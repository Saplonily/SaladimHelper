﻿local t = {}
t.name = "SaladimHelper/StaticFilterTrigger"
t.placements = {
    name = "normal",
    data = {
        width = 16,
        height = 16,
        strength = 100.0,
        effect_path = "SaladimHelper/Blur",
        index = 0.0
    }
}

t.fieldInformation = {
    effect_path = {
        options = require("mods").requireFromPlugin("libraries.filters"),
        editable = true
    }
}

return t