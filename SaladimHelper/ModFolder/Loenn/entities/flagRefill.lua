local refill = {}

refill.name = "SaladimHelper/FlagRefill"
refill.depth = -100
refill.placements = {
    {
        name = "normal",
        data = {
            idleTexture = "objects/refill/idle",
            outlineTexture = "objects/refill/outline",
            flashTexture = "objects/refill/flash",
            oneUse = false,
            dashRefill = true,
            flag = "",
            useOnlyNoFlag = false,
            sfxUsed = "event:/game/general/diamond_touch",
            sfxRespawned = "event:/game/general/diamond_return",
            removeFlagDelay = 0.2
        }
    }
}

function refill.texture(room, entity)
    return entity.idleTexture .. "00"
end

return refill