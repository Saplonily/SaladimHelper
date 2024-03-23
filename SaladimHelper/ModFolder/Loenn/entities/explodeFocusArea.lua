local area = {}

area.name = "SaladimHelper/ExplodeFocusArea"
area.depth = 6000
area.fillColor = { 0.7294, 0.8745, 0.9568, 0.2 }
area.borderColor = { 1.0, 1.0, 1.0 }
area.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        focusType = 1,
        rotation = 0
    }
}

area.fieldInformation = {
    focusType = {
        options = {
            ["EightWay"] = 0,
            ["FourWay"] = 1,
            ["DiagonalFourWay"] = 2,
            ["HorizontalTwoWay"] = 3,
            ["VerticalTwoWay"] = 4,
        },
        editable = false,
        fieldType = "integer"
    }
}

return area
