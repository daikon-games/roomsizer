using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roomsizer {
    class RoomResizer {

        public static JObject ResizeRoom(JObject roomJson, int newWidth, int newHeight, int tileSize, MainPage.AnchorDirection anchorDirection) {
            int oldWidth = (int) roomJson["roomSettings"]["Width"];
            int oldHeight = (int) roomJson["roomSettings"]["Height"];
            if (newHeight < oldHeight || newWidth < oldWidth) {
                throw new ArgumentOutOfRangeException();
            }
            newWidth = (int) Math.Ceiling((double) newWidth / tileSize) * tileSize;
            newHeight = (int) Math.Ceiling((double)newHeight / tileSize) * tileSize;
            roomJson["roomSettings"]["Width"] = newWidth;
            roomJson["roomSettings"]["Height"] = newHeight;

            var widthDiff = newWidth - oldWidth;
            var heightDiff = newHeight - oldHeight;

            foreach (var layer in roomJson["layers"]) {
                if (layer["assets"] != null) {
                    // Move assets
                    foreach(var asset in layer["assets"]) {
                        if (new[] { MainPage.AnchorDirection.T, MainPage.AnchorDirection.C, MainPage.AnchorDirection.B }.Contains(anchorDirection)) {
                            // central anchor column, add half the new width difference to X
                            asset["x"] = (int) asset["x"] + (widthDiff / 2);
                        } else if (new[] { MainPage.AnchorDirection.TR, MainPage.AnchorDirection.R, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                            // right anchor column, add the whole new width difference to X
                            asset["x"] = (int)asset["x"] + widthDiff;
                        }
                        if (new[] { MainPage.AnchorDirection.L, MainPage.AnchorDirection.C, MainPage.AnchorDirection.R }.Contains(anchorDirection)) {
                            // central anchor row, add half the new height difference to Y
                            asset["y"] = (int)asset["y"] + (heightDiff / 2);
                        } else if (new[] { MainPage.AnchorDirection.BL, MainPage.AnchorDirection.B, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                            // bottom anchor row, add the whole new height difference to Y
                            asset["y"] = (int)asset["y"] + heightDiff;
                        }
                    } 
                } else if (layer["instances"] != null) {
                    // Move instances
                    foreach (var inst in layer["instances"]) {
                        if (new[] { MainPage.AnchorDirection.T, MainPage.AnchorDirection.C, MainPage.AnchorDirection.B }.Contains(anchorDirection)) {
                            // central anchor column, add half the new width difference to X
                            inst["x"] = (int)inst["x"] + (widthDiff / 2);
                        } else if (new[] { MainPage.AnchorDirection.TR, MainPage.AnchorDirection.R, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                            // right anchor column, add the whole new width difference to X
                            inst["x"] = (int)inst["x"] + widthDiff;
                        }
                        if (new[] { MainPage.AnchorDirection.L, MainPage.AnchorDirection.C, MainPage.AnchorDirection.R }.Contains(anchorDirection)) {
                            // central anchor row, add half the new height difference to Y
                            inst["y"] = (int)inst["y"] + (heightDiff / 2);
                        } else if (new[] { MainPage.AnchorDirection.BL, MainPage.AnchorDirection.B, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                            // bottom anchor row, add the whole new height difference to Y
                            inst["y"] = (int)inst["y"] + heightDiff;
                        }
                    }
                } else if (layer["tiles"] != null) {
                    // Handle tile layers
                    layer["tiles"] = MoveTiles(layer["tiles"], oldWidth, oldHeight, newWidth, newHeight, anchorDirection);
                }
            }

            return roomJson;
        }

        private static JToken MoveTiles(JToken tileLayer, int oldWidth, int oldHeight, int newWidth, int newHeight, MainPage.AnchorDirection anchorDirection) {
            var oldTileColCount = (int)tileLayer["SerialiseWidth"];
            var oldTileRowCount = (int)tileLayer["SerialiseHeight"];
            var tileSizeX = (oldWidth - 1) / oldTileColCount + 1;
            var tileSizeY = (oldHeight - 1) / oldTileRowCount + 1;
            tileLayer["SerialiseWidth"] = (newWidth - 1) / tileSizeX + 1;
            tileLayer["SerialiseHeight"] = (newHeight - 1) / tileSizeY + 1;
            var newTileColCount = (int)tileLayer["SerialiseWidth"];
            var newTileRowCount = (int)tileLayer["SerialiseHeight"];

            // We need to insert this many new rows/columns of blank tiles to the top, bottom, and/or sides, depending on anchor direction
            var colsToAdd = newTileColCount - oldTileColCount;
            var rowsToAdd = newTileRowCount - oldTileRowCount;

            var tileGrid = new TileGrid((JArray) tileLayer["TileSerialiseData"], oldTileColCount);
            if (colsToAdd != 0) {
                Debug.WriteLine("Adding " + colsToAdd + " cols");
                if (new[] { MainPage.AnchorDirection.T, MainPage.AnchorDirection.C, MainPage.AnchorDirection.B }.Contains(anchorDirection)) {
                    // central anchor column, add half the new columns on the left, and half on the right
                    foreach (var tileRow in tileGrid.RowList) {
                        var i = 0;
                        // first half of new columns going on the left
                        for (; i < Math.Floor(colsToAdd / 2f); i++) {
                            tileRow.Tiles.Insert(0, 0);
                        }
                        // second half of new columns going on the right
                        for (; i < colsToAdd; i++) {
                            tileRow.Tiles.Add(0);
                        }
                    }
                } else if (new[] { MainPage.AnchorDirection.TR, MainPage.AnchorDirection.R, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                    // right anchor column, add all the new columns on the left
                    foreach (var tileRow in tileGrid.RowList) {
                        for (var i = 0; i < colsToAdd; i++) {
                            tileRow.Tiles.Insert(0, 0);
                        }
                    }
                } else {
                    // left anchor column, add all the new columns on the right
                    foreach (var tileRow in tileGrid.RowList) {
                        for (var i = 0; i < colsToAdd; i++) {
                            tileRow.Tiles.Add(0);
                        }
                    }
                }
            }
            if (rowsToAdd != 0) {
                Debug.WriteLine("Adding " + rowsToAdd + " rows");
                if (new[] { MainPage.AnchorDirection.L, MainPage.AnchorDirection.C, MainPage.AnchorDirection.R }.Contains(anchorDirection)) {
                    // central anchor row, add half the new rows on top and half on the bottom
                    var i = 0;
                    for (; i < Math.Floor(rowsToAdd / 2f); i++) {
                        // first half of new rows going on top
                        tileGrid.RowList.Insert(0, new TileGrid.TileRow(Enumerable.Repeat(0, newTileColCount).ToList()));
                    }
                    for (; i < rowsToAdd; i++) {
                        // second half of new rows going on top
                        tileGrid.RowList.Add(new TileGrid.TileRow(Enumerable.Repeat(0, newTileColCount).ToList()));
                    }
                } else if (new[] { MainPage.AnchorDirection.BL, MainPage.AnchorDirection.B, MainPage.AnchorDirection.BR }.Contains(anchorDirection)) {
                    // bottom anchor row, add all the new rows to the top
                    for (var i = 0; i < rowsToAdd; i++) {
                        tileGrid.RowList.Insert(0, new TileGrid.TileRow(Enumerable.Repeat(0, newTileColCount).ToList()));
                    }
                } else {
                    // top anchor row, add all the new rows to the bottom
                    for (var i = 0; i < rowsToAdd; i++) {
                        tileGrid.RowList.Add(new TileGrid.TileRow(Enumerable.Repeat(0, newTileColCount).ToList()));
                    }
                }
            }
            tileLayer["TileSerialiseData"] = JArray.FromObject(tileGrid.SerializeTileData(newTileColCount, newTileRowCount));

            return tileLayer;
        }

    }
}
