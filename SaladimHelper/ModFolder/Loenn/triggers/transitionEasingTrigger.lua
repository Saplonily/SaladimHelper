local t = {}

t.name = "SaladimHelper/TransitionEasingTrigger"

t.placements = {
    name = "normal",
    data = {
        width = 16,
        height = 16,
        easing = "CubeOut",
        threshold = 0.9,
        speed = 60,
        duration = 0.65
    }
}

t.fieldInformation = {
    easing = {
        options = require("mods").requireFromPlugin("libraries.easing_enums"),
        editable = false
    }
}

return t