var app = angular.module('PayrollApp', ['ui.bootstrap']);

//try to refactor this sometime to remove the $ctrl
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

app.controller('ModalInstanceCtrl', function ($uibModalInstance, $http, responseData, $compile) {
    var $ctrl = this;

    $ctrl.error = [];
    $ctrl.regularHours = responseData.data.EmployeeTotalHoursViewModel.RegularHours;
    $ctrl.overtime = responseData.data.EmployeeTotalHoursViewModel.Overtime;
    $ctrl.nightDifferential = responseData.data.EmployeeTotalHoursViewModel.NightDifferential;
    $ctrl.modalTitle = responseData.data.ModalTitle;
    $ctrl.companies = responseData.data.Companies;
    $ctrl.regularHoursPerCompany = responseData.data.RegularHoursPerCompany;
    $ctrl.overtimePerCompany = responseData.data.OvertimePerCompany;
    $ctrl.nightDifferentialPerCompany = responseData.data.NightDifferentialPerCompany;    

    $ctrl.regularId = responseData.data.EmployeeTotalHoursViewModel.TotalRegularHoursId;
    $ctrl.overtimeId = responseData.data.EmployeeTotalHoursViewModel.TotalOvertimeId;
    $ctrl.nightDifferentialId = responseData.data.EmployeeTotalHoursViewModel.TotalNightDifferentialId;

    $ctrl.addCompany = function (array, id) {
        array.push({
            TotalEmployeeHoursPerCompanyId: 0,
            TotalEmployeeHoursId: id,
            CompanyId: 0,
            Hours: 0
        });
    }

    $ctrl.save = function () {

        if (!$ctrl.validate())
            return;        

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
            success: function(response) {
                if (response.Success === true) {
                    $uibModalInstance.dismiss('cancel');
                } else {
                    alert(response.Error);
                }
            }
        });
    };

    $ctrl.validate = function () {
        $ctrl.error = [];
        var regularHoursValid = $ctrl.TotalHoursIsValid($ctrl.regularHoursPerCompany, $ctrl.regularHours, 'Regular hours');
        var overtimeValid = $ctrl.TotalHoursIsValid($ctrl.overtimePerCompany, $ctrl.overtime, 'Overtime');
        var nightDiffValid = $ctrl.TotalHoursIsValid($ctrl.nightDifferentialPerCompany, $ctrl.nightDifferential, 'Night Differential');

        return regularHoursValid && overtimeValid && nightDiffValid;
    }

    $ctrl.TotalHoursIsValid = function (array, totalHours, title) {
        var valid = true;

        var sum = 0;
        angular.forEach(array, function (value) {
            sum = sum + parseFloat(value.Hours);
        });

        if (sum > parseFloat(totalHours)) {
            $ctrl.error.push('Total ' + title + ' is greater than actual hours');
            valid = false;
        }
        if (sum < parseFloat(totalHours)) {
            $ctrl.error.push('Total ' + title + ' is less than actual hours');
            valid = false;
        }

        return valid;
    }

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.remove = function (array, item) {
        var index = array.indexOf(item);
        array.splice(index, 1);
    }
});

app.directive('errorMessage', function ($compile) {
    return {
        restrict: 'E',
        replace: true,
        template: '<div class="alert alert-danger fade in" ng-show="$ctrl.error.length > 0"><div>' +
                  '<ul><li ng-repeat="e in $ctrl.error">{{e}}</li></ul>' +
                  '</div></div>'
    }
});