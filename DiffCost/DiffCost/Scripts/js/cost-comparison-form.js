'use strict';

(function () {
	angular.module('costComparisonFormApp', []).directive('costComparisonForm', function () {
		// templateUrlは、ディレクティブを呼び出しているページからの相対パス
		// ルートからの相対パスにして、どこからでも使えるようにする
		return {
			restrict: 'E',
			templateUrl: '/Pages/cost-comparison-form.html',
			controller: ['$http', function ($http) {
				// ケアレスミスを防ぐため、最初にthisを移し替えることをルールとしたほうがいいのかも
				var thisController = this;

				// ユーザーをひも付ける情報は、railsのidとアプリのidとどちらがよいか
				// データベースの移行を考えると、連番よりも固有のアカウントIDのほうがよさそうに感じる
				// API越しに関連付ける際、デフォルトのUser.idではなく、User.user_idとひも付ける
				thisController.pageTitleDefault = "見積・実績比較";
				thisController.pageTitle = thisController.pageTitleDefault;
				thisController.projects = [];
				thisController.costComparisonResult = {};
				thisController.userId = "";
				thisController.waiting = false;
				thisController.csvUrl = "";
				thisController.quotationsTerm = { start: "", end: "" };

				// プロジェクトの種類を取得
				$http.get('/api/Quotations/?projectgroup=').success(function (data) {
					thisController.projects = data;
					thisController.selectedProject = thisController.projects[0].ProjectName;
					thisController.changedSelectedProject();
				});

				// プロパティもメソッドも全部定義するから、中身が膨れやすい
				this.submitForm = function () {
					thisController.pageTitle = "取得中... : ";

					// データを取得
					thisController.waiting = true;
					var project_name_encoded = encodeURIComponent(thisController.selectedProject);
					$http.get("/api/CostComparisons/?projectname=" + project_name_encoded).success(function (data, status, headers, config) {
						// タイトルを元に戻し、データを保持
						thisController.pageTitle = thisController.pageTitleDefault;
						thisController.costComparisonResult = data;
						thisController.waiting = false;
					}).error(function (data, status, headers, config) {
						thisController.pageTitle = "取得失敗";
						thisController.costComparisonResult = data;
						thisController.waiting = false;
					});
				};

				// TODO: 画面のラベルに表示する文字列はコントローラーの責務でないように思えるが、一旦実装する
				this.getQuotationTermText = function () {
					return this.getTermText(this.costComparisonResult.quotations);
				};

				this.getFactTermText = function () {
					return this.getTermText(this.costComparisonResult.facts);
				};

				this.getTermText = function (dataArray) {
					var quotationTermText = "";
					if (dataArray && dataArray.length > 0) {
						// 分単位で表示（対応ブラウザのみ）
						var options = { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
						quotationTermText = new Date(dataArray[0].UpdatedAt).toLocaleString('ja-JP', options).replace("JST", "") + " 〜 " + new Date(dataArray[dataArray.length - 1].UpdatedAt).toLocaleString('ja-JP', options).replace("JST", "");
					}

					return quotationTermText;
				};

				this.getProjectNameText = function () {
					var dataArray = this.costComparisonResult.quotations || this.costComparisonResult.facts;
					return dataArray && dataArray.length > 0 ? dataArray[0].ProjectName : "";
				};

				this.getTotalManDayText = function () {
					var mandayQ = this.getTotalManDayOfQuotations();
					var mandayF = this.getTotalManDayOfFacts();
					return !mandayQ && !mandayF ? "" : '見積：' + mandayQ + '\b／ 実績：' + mandayF;
				};

				this.getTotalManDayOfQuotations = function () {
					return this.getTotalManDay(this.costComparisonResult.quotations);
				};

				this.getTotalManDayOfFacts = function () {
					return this.getTotalManDay(this.costComparisonResult.facts);
				};

				this.getTotalManDay = function (array) {
					// TODO: 小数点の扱い 見た目上の合計と差異が出る 12.9 <=> 12.89999...
					return !array ? "" : array.reduce(function (prevValue, currentValue) {
						return prevValue + currentValue.ManDay;
					}, 0);
				};

				this.getPostData = function () {
					return {
						'url': '/api/CostComparisons',
						'params': thisController.getPostParams()
					};
				};

				this.getPostParams = function () {
					var postParam = {
						'ProjectName': thisController.selectedProject,
						'UserAccountId': thisController.userTableId
					};

					return postParam;
				};

				this.downloadCsv = function () {
					thisController.pageTitle = "CSV取得中... : ";

					// 実績比較表取得のためのデータを取得
					//var postData = thisController.getPostData();

					// 取得
					// TODO:どういうリクエストを送ればいい？
					thisController.waiting = true;
					var project_name_encoded = encodeURIComponent(thisController.selectedProject);
					$http.get("/api/CostComparisons/download/?format=csv&projectnamecsv=" + project_name_encoded).success(function (data, status, headers, config) {
						thisController.pageTitle = "取得完了";
						thisController.waiting = false;
					}).error(function (data, status, headers, config) {
						thisController.pageTitle = "取得失敗";
						thisController.waiting = false;
					});
				};

				this.changedSelectedProject = function () {
					var project_name_encoded = encodeURIComponent(thisController.selectedProject);
					thisController.csvUrl = "/api/CostComparisons/download/?format=csv&projectnamecsv=" + project_name_encoded;
				};
			}],
			controllerAs: 'costComparison'
		};
	});
})();
//# sourceMappingURL=cost-comparison-form.js.map