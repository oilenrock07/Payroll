var app = angular.module('PayrollApp', ['ui.bootstrap']);

app.controller('ModalController', function ($scope, $http, $uibModal, $log, $document, $compile) {
    var $ctrl = this;
    $ctrl.animationsEnabled = true;

    $ctrl.showModal = function (responseData) {
        var parentElem = angular.element($document[0].querySelector('.hours-per-company-modal'));
        var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'hoursPerCompanyModal.html',
            controller: 'ModalInstanceCtrl',
            controllerAs: '$ctrl',
            keyboard: false,
            backdrop: 'static',
            size: 'lg',
            appendTo: parentElem,
            resolve: {
                responseData: function () {
                    return responseData;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $ctrl.selected = selectedItem;
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $ctrl.open = function (employeeId, date) {
        $http({
            method: 'POST',
            url: '/Attendance/ViewHoursPerCompanyModal',
            params: { employeeId: employeeId, date: date }
        }).then($ctrl.showModal);
    };

    $scope.activateView = function (html) {
        $compile(html.contents())($scope);

        if (!$scope.$$phase)
            $scope.$apply();
    };

    $ctrl.search = function () {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        var employeeId = $('.js-employeeIdTypeAhead').val();
        if (employeeId == '' || employeeId == undefined) {
            employeeId = 0;
        }

        $http({
            method: 'GET',
            url: '/Attendance/HoursPerCompanyContent',
            params: { startDate: startDate, endDate: endDate, employeeId: parseInt(employeeId) }
        }).then(function (responseData) {

            var content = $('#content');
            content.html(responseData.data);

            var mController = angular.element(document.getElementsByClassName("hours-per-company-modal"));
            mController.scope().activateView(content);

        });
    };
});

app.controller('ModalInstanceCtrl', function ($uibModalInstance, $http, responseData) {
    var $ctrl = this;

    $ctrl.regularHours = responseData.data.EmployeeTotalHoursViewModel.RegularHours;
    $ctrl.overtime = responseData.data.EmployeeTotalHoursViewModel.Overtime;
    $ctrl.nightDifferential = responseData.data.EmployeeTotalHoursViewModel.NightDifferential;
    $ctrl.modalTitle = responseData.data.ModalTitle;
    $ctrl.companies = responseData.data.Companies;
    $ctrl.regularHoursPerCompany = responseData.data.RegularHoursPerCompany;
    $ctrl.overtimePerCompany = responseData.data.OvertimePerCompany;
    $ctrl.nightDifferentialPerCompany = responseData.data.NightDifferentialPerCompany;

    $ctrl.addCompany = function (array, item) {
        array.push({
            TotalEmployeeHoursPerCompanyId: 0,
            TotalEmployeeHoursId: item.TotalEmployeeHoursPerCompanyId,
            CompanyId: 0,
            Hours: 0
        });
    }

    $ctrl.save = function () {

        var viewModel = {
            RegularHoursPerCompany: $ctrl.regularHoursPerCompany,
            OvertimePerCompany: $ctrl.overtimePerCompany,
            NightDifferentialPerCompany: $ctrl.nightDifferentialPerCompany
        };

        $.ajax({
            type: 'POST',
            url: '/Attendance/CreateHoursPerCompany',
            contentType: 'application/json',
            data: JSON.stringify(viewModel),
            success: function() {

            }
        });
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.remove = function (array, item) {
        var index = array.indexOf(item);
        array.splice(index, 1);
    }
});