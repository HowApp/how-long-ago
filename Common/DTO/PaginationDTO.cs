namespace How.Common.DTO;

public class PaginationDTO
{
    private int _page;
    private int _size;

    public int Page
    {
        get => _page;
        set
        {
            if (value < 1 )
            {
                _size = 1;
            }
        }
    }

    public int Size
    {
        get => _size;
        set
        {
            _size = value switch
            {
                > 100 => 100,
                < 1 => 1,
                _ => _size
            };
        }
    }
}