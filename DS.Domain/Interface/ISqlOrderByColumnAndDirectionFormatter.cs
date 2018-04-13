namespace DS.Domain.Interface
{
    public interface ISqlOrderByColumnAndDirectionFormatter
    {
        string CheckOrderByDirection(DataSetSchema schema, string orderDirection);
        string CheckOrderByColumn(DataSetSchema schema, string orderByColumn);
    }
}