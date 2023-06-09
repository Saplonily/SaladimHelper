local rustyZipMover = {}

rustyZipMover.name = "SaladimHelper/RustyZipMover"
rustyZipMover.depth = 8990
rustyZipMover.minimumSize = {16, 16}
rustyZipMover.nodeLimits = {1, 1}
rustyZipMover.nodeVisibility = "selected"
rustyZipMover.nodeLineRenderType = "line"
rustyZipMover.placements = {
    name = "rustyZipMover_normal",
    data = {
        width = 16,
        height = 16,
        is_moon = false
    }
}

return rustyZipMover
