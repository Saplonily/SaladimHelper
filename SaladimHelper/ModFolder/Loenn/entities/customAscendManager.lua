local entity = {}

entity.name = "SaladimHelper/CustomAscendManager"
entity.depth = 0
entity.texture = "@Internal@/summit_background_manager"
entity.fieldInformation = {
	backgroundColor = {
        fieldType = "color"
    }
}

entity.placements = {
    name = "normal",
    data = {
        backgroundColor = "75A0AB",
        streakColors = "FFFFFF,E69ECB",
	dialog = "",
	introLaunch = false,
	finalLaunch = false,
	borders = true,
	dark = false
    }
}

return entity