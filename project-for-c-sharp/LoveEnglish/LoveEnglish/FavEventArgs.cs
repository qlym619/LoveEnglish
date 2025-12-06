using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveEnglish {
    internal class FavEventArgs : EventArgs {
        private bool Flag { get; }
        public int WordId { get; }

        public FavEventArgs(int wordId) {
            WordId = wordId;
        }

        public FavEventArgs(bool flag) {
            Flag = flag;
        }
    }
}
