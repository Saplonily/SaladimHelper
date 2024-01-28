local entity = {}

local sides = {
    A = "A",
    B = "B",
    C = "C"
}

entity.name = "SaladimHelper/FallTeleportMirror"
entity.texture = "objects/temple/portal/portalframe"
entity.justification = { 0.5, 0.5 }
entity.fieldInformation = {
    Side = {
        options = sides,
        editable = false
    }
}

entity.placements = {
    {
        name = "roomTeleport",
        data = {
            toLevel = "",
            side = "A",
            frameSprite = "",
            stopMusic = true,
            fadeParameter = "fade"
        }
    },
    {
        name = "endLevel",
        data = {
            toLevel = "",
            endLevel = true,
            side = "A",
            frameSprite = "",
            stopMusic = true,
            fadeParameter = "fade"
        }
    },
    {
        name = "chapterChange",
        data = {
            toLevel = "",
            mapSID = "Celeste/LostLevels",
            seamlessNextChapter = true,
            side = "A",
            frameSprite = "",
            stopMusic = true,
            fadeParameter = "fade"
        }
    }
}


return entity
