using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RentalApp.ViewModels;

public class UserListViewModel : BaseViewModel
{
    public ObservableCollection<UserListItem> FilteredUsers { get; set; }
        = new ObservableCollection<UserListItem>();

    public string SearchText { get; set; }
    public List<string> RoleFilterOptions { get; set; } = new() { "All", "Admin", "User" };
    public string SelectedRoleFilter { get; set; }

    public bool IsRefreshing { get; set; }
    public bool IsLoading { get; set; }

    public ICommand RefreshCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand NavigateToDashboardCommand { get; }
    public ICommand UserSelectedCommand { get; }

    public UserListViewModel()
    {
        RefreshCommand = new Command(OnRefresh);
        CreateUserCommand = new Command(OnCreateUser);
        NavigateToDashboardCommand = new Command(OnNavigateHome);
        UserSelectedCommand = new Command<UserListItem>(OnUserSelected);
    }

    void OnRefresh() { }
    void OnCreateUser() { }
    void OnNavigateHome() { }
    void OnUserSelected(UserListItem user) { }
}
