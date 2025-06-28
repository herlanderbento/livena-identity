namespace Livena.Identity.Api.Presenters;

public class ApiPresenter<TData>
{
    public TData Data { get; private set; }

    public ApiPresenter(TData data) 
        => Data = data;
}