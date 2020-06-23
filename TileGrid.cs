using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace Roomsizer {
    class TileGrid {

        public class TileRow {
            public List<Int32> Tiles { get; set; }

            public TileRow(List<Int32> tiles) => Tiles = tiles;            
        }

        public List<TileRow> RowList { get; set; }

        public TileGrid(JArray rawTileData, int colCount) {
            RowList = new List<TileRow>();
            int tileCounter = 0;
            var tiles = new List<Int32>();
            foreach(var tile in rawTileData.Children()) {
                tiles.Add((int) tile);
                tileCounter += 1;
                if (tileCounter == colCount) {
                    RowList.Add(new TileRow(tiles));
                    tileCounter = 0;
                    tiles = new List<Int32>();
                }
            }
        }

        public int[] SerializeTileData(int colCount, int rowCount) {
            var result = new int[colCount * rowCount];
            var i = 0;
            foreach(var row in RowList) {
                foreach(int tile in row.Tiles) {
                    result[i++] = tile;
                }
            }
            return result;
        }

    }
}
