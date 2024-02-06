local windSnow = {}

windSnow.name = "SaladimHelper/GravityWindSnowFG"
windSnow.fieldInformation = {
    color = {
        fieldType = "color",
        allowEmpty = true
    },
    speedMul = {
        fieldType = "number",
        allowEmpty = false
    }
}
windSnow.defaultData = {
    color = "",
    speedMul = 1.0
}

windSnow.canBackground = false
windSnow.canForeground = true

return windSnow