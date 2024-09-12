namespace How.Common.DTO;

public class PaginationDTO
{
    private int _page = 1;
    private int _size = 20;
    private static int _maxSize = 100;

    public int Page
    {
        get => _page;
        set
        {
            if (value < 1 )
            {
                _page = 1;
            }
            
            _page = value;
        }
    }

    public int Size
    {
        get => _size;
        set
        {
            _size = value switch
            {
                > 100 => _maxSize,
                < 1 => 1,
                _ => value
            };
        }
    }

    public static int MaxSize => _maxSize;
}