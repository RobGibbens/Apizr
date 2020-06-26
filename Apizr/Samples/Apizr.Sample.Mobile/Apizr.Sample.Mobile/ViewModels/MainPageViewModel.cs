﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Apizr.Mediation.Cruding;
using Apizr.Requesting;
using Apizr.Sample.Api;
using Apizr.Sample.Api.Models;
using MediatR;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using Color = System.Drawing.Color;

namespace Apizr.Sample.Mobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IApizrManager<IHttpBinService> _httpBinManager;
        private readonly IApizrManager<IReqResService> _reqResManager;
        private readonly IApizrManager<ICrudApi<User, int, PagedResult<User>>> _userCrudManager;
        private readonly IApizrManager<ICrudApi<UserDetails, int, IEnumerable<UserDetails>>> _userDetailsCrudManager;
        private readonly IMediator _mediator;

        public MainPageViewModel(INavigationService navigationService,
            IApizrManager<IHttpBinService> httpBinManager, IApizrManager<IReqResService> reqResManager,
            IApizrManager<ICrudApi<User, int, PagedResult<User>>> userCrudManager, 
            IApizrManager<ICrudApi<UserDetails, int, IEnumerable<UserDetails>>> userDetailsCrudManager,
            IMediator mediator)
            : base(navigationService)
        {
            _httpBinManager = httpBinManager;
            _reqResManager = reqResManager;
            _userCrudManager = userCrudManager;
            _userDetailsCrudManager = userDetailsCrudManager;
            _mediator = mediator;
            GetUsersCommand = ExecutionAwareCommand.FromTask(GetUsersAsync)
                .OnIsExecutingChanged(isExecuting => IsRefreshing = isExecuting);
            //GetUserDetailsCommand = ExecutionAwareCommand.FromTask<User>(GetUserDetails);
            GetUserDetailsCommand = new DelegateCommand<User>(async user => await GetUserDetails(user));
            AuthCommand = ExecutionAwareCommand.FromTask(AuthAsync);

            Title = "Main Page";
        }

        #region Properties

        [Reactive] public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        [Reactive] public bool IsRefreshing { get; set; }

        public ICommand GetUsersCommand { get; }

        public ICommand GetUserDetailsCommand { get; }

        public ICommand AuthCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Managing crud entities comes with 3 web api call flavors
        /// Choosing one of it depends of your registration settings
        /// and how you like to play with api calls
        /// </summary>
        /// <returns></returns>
        private async Task GetUsersAsync()
        {
            if (IsRefreshing)
                return;

            IList<User>? users;
            try
            {
                // This is a manually defined web api call into IReqResService (classic actually)
                //var userList = await _reqResManager.ExecuteAsync((ct, api) => api.GetUsersAsync(ct), CancellationToken.None);
                //users = userList.Data;

                // This is the Crud way, with or without Crud attribute auto registration, but without mediation
                //var pagedUsers = await _userCrudManager.ExecuteAsync((ct, api) => api.ReadAll(ct), CancellationToken.None);
                //users = pagedUsers.Data?.ToList();

                // The same as before but with auto mediation handling
                var pagedUsers = await _mediator.Send(new ReadAllQuery<PagedResult<User>>(), CancellationToken.None);
                users = pagedUsers.Data?.ToList();
            }
            catch (ApizrException<UserList> e)
            {
                var message = e.InnerException is IOException ? "No network" : (e.Message ?? "Error");
                UserDialogs.Instance.Toast(new ToastConfig(message) { BackgroundColor = Color.Red, MessageTextColor = Color.White });

                users = e.CachedResult?.Data;
            }

            if (users != null)
                Users = new ObservableCollection<User>(users);
        }

        /// <summary>
        /// Managing crud entities comes with 3 web api call flavors
        /// Choosing one of it depends of your registration settings
        /// and how you like to play with api calls.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task GetUserDetails(User user)
        {
            User? fetchedUser;
            try
            {
                // This is a manually defined web api call into IReqResService (classic actually)
                //var userDetails = await _reqResManager.ExecuteAsync((ct, api) => api.GetUserAsync(user.Id, ct), CancellationToken.None);

                // This is the Crud way, with or without Crud attribute auto registration, but without mediation
                //var userDetails = await _userDetailsCrudManager.ExecuteAsync((ct, api) => api.Read(user.Id, ct), CancellationToken.None);

                // The same as before but with auto mediation handling
                var userDetails = await _mediator.Send(new ReadQuery<UserDetails, int>(user.Id), CancellationToken.None);
                fetchedUser = userDetails?.User;
            }
            catch (ApizrException<UserDetails> e)
            {
                var message = e.InnerException is IOException ? "No network" : (e.Message ?? "Error");
                UserDialogs.Instance.Toast(new ToastConfig(message)
                    {BackgroundColor = Color.Red, MessageTextColor = Color.White});

                fetchedUser = e.CachedResult?.User;
            }

            if (fetchedUser != null)
                UserDialogs.Instance.Alert(
                    $"{fetchedUser.FirstName} {fetchedUser.LastName}\n {fetchedUser.Email}", fetchedUser.FirstName);
        }

        private async Task AuthAsync()
        {
            string result;
            var succeed = false;
            try
            {
                var response = await _httpBinManager.ExecuteAsync(api => api.AuthBearerAsync());
                succeed = response.IsSuccessStatusCode;
                result = response.IsSuccessStatusCode ? "Authentication succeed :)" : "Authentication failed :(";
            }
            catch (ApizrException e)
            {
                result = e.InnerException is IOException ? "No network" : (e.Message ?? "Error");
            }

            if (!string.IsNullOrWhiteSpace(result))
                UserDialogs.Instance.Toast(new ToastConfig(result)
                    {BackgroundColor = succeed ? Color.Green : Color.Red, MessageTextColor = Color.White});
        }

        #endregion

        #region Lifecycle

        public override void OnAppearing()
        {
            base.OnAppearing();

            GetUsersCommand.Execute(null);
        }

        #endregion
    }
}
