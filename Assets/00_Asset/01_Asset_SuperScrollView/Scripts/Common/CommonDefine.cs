namespace _00_Asset._01_Asset_SuperScrollView.Scripts.Common
{

    public enum SnapStatus
    {
        NoTargetSet = 0,
        TargetHasSet = 1,
        SnapMoving = 2,
        SnapMoveFinish = 3
    }


    public enum ItemCornerEnum
    {
        LeftBottom = 0,
        LeftTop,
        RightTop,
        RightBottom,
    }


    public enum ListItemArrangeType
    {
        TopToBottom = 0,
        BottomToTop,
        LeftToRight,
        RightToLeft,
    }

    public enum GridItemArrangeType
    {
        TopLeftToBottomRight = 0,
        BottomLeftToTopRight,
        TopRightToBottomLeft,
        BottomRightToTopLeft,
    }
    public enum GridFixedType
    {
        ColumnCountFixed = 0,
        RowCountFixed,
    }

    public struct RowColumnPair
    {
        public RowColumnPair(int row1, int column1)
        {
            MRow = row1;
            MColumn = column1;
        }

        public bool Equals(RowColumnPair other)
        {
            return this.MRow == other.MRow && this.MColumn == other.MColumn;
        }

        public static bool operator ==(RowColumnPair a, RowColumnPair b)
        {
            return (a.MRow == b.MRow)&&(a.MColumn == b.MColumn);
        }
        public static bool operator !=(RowColumnPair a, RowColumnPair b)
        {
            return (a.MRow != b.MRow) || (a.MColumn != b.MColumn); ;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return (obj is RowColumnPair) && Equals((RowColumnPair)obj);
        }


        public int MRow;
        public int MColumn;
    }
}
