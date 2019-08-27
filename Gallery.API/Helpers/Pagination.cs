namespace Gallery.API.Helpers
{
    public class Pagination
    {
        public int Skip
        {
            get
            {
                int tmpSkip = PageSize * (PageNumber - 1);

                if (tmpSkip > int.MaxValue)
                {
                    return int.MaxValue;
                }
                else if (tmpSkip < 0)
                {
                    return 0;
                }
                else
                {
                    return tmpSkip;
                }
            }
        }

        public int Take
        {
            get
            {
                return PageSize;
            }
        }

        public const int maxPageNumber = int.MaxValue;
        public const int minimumPageNumber = 1;
        private int _pageNumber = minimumPageNumber;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value > maxPageNumber)
                {
                    _pageNumber = maxPageNumber;
                }
                else if (value < minimumPageNumber)
                {
                    _pageNumber = minimumPageNumber;
                }
                else
                {
                    _pageNumber = value;
                }
            }
        }

        public const int maxPageSize = 20;
        public const int minimumPageSize = 10;
        private int _pageSize = minimumPageSize;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value > maxPageSize)
                {
                    _pageSize = maxPageSize;
                }
                else if (value < minimumPageSize)
                {
                    _pageSize = minimumPageSize;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }
    }
}
