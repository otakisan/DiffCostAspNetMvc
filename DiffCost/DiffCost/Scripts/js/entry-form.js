'use strict';

(function () {
	angular.module('entryFormApp', []).directive('entryForm', function () {
		// templateUrlは、ディレクティブを呼び出しているページからの相対パス
		// ルートからの相対パスにして、どこからでも使えるようにする
		return {
			restrict: 'E',
			templateUrl: '/Pages/entry-form.html',
			controller: ['$http', function ($http) {
				// ケアレスミスを防ぐため、最初にthisを移し替えることをルールとしたほうがいいのかも
				var thisController = this;

				// ユーザーをひも付ける情報は、railsのidとアプリのidとどちらがよいか
				// データベースの移行を考えると、連番よりも固有のアカウントIDのほうがよさそうに感じる
				// API越しに関連付ける際、デフォルトのUser.idではなく、User.user_idとひも付ける
				thisController.appTitle = "見積・実績";
				thisController.pageTitle = "入力ページ(兼トップページ)";
				thisController.userText = "";
				thisController.userTableId = 0;
				thisController.userId = "";
				thisController.entityTargets = [{
					'name': '見積',
					'value': 'Quotation'
				}, {
					'name': '実績',
					'value': 'Fact'
				}];
				thisController.entityTarget = thisController.entityTargets[0].value;

				// 通信がからむときの実行順が気になる。ここに限らずだけど。
				// TODO: ユーザー固定の仕様だが、特定ユーザーを検索する形にし、存在しなければ登録できないようにするか
				$http.get('/api/Users/1').success(function (data) {
					thisController.userTableId = data.Id;
					thisController.userId = data.UserAccountId;
				});

				// プロパティもメソッドも全部定義するから、中身が膨れやすい
				this.submitForm = function () {
					thisController.pageTitle = "登録中... : " + thisController.userText;

					// 選択したエンティティ向けのパラメータを作成
					var postData = thisController.getPostData();

					// 登録
					$http.post(postData.url, postData.params).success(function (data, status, headers, config) {
						thisController.pageTitle = "登録完了 ID：" + data.Id;
						thisController.userText = "";
					}).error(function (data, status, headers, config) {
						thisController.pageTitle = "登録失敗";
					});
				};

				this.getPostData = function () {
					return {
						'url': '/api/' + thisController.entityTarget + 's',
						'params': thisController.getPostParams()
					};
				};

				this.getPostParams = function () {
					var textPropName = thisController.entityTarget + 'Text';
					var projectNameAndText = thisController.splitProjectNameAndText();
					var postParam = {
						'ProjectName': projectNameAndText.project_name,
						'UserAccountId': thisController.userTableId,
						'ManDay': projectNameAndText.man_day
					};
					postParam[textPropName] = projectNameAndText.text;

					return postParam;
				};

				this.splitProjectNameAndText = function () {
					// Javascriptでは、後読みがサポートされていない
					// #始まりはプロジェクト案件名、空白で区切って、以降は全て内容とする
					var re = /(^#.+?(?=\s+))?(.+)/g;
					var matchArray = re.exec(thisController.userText.trim());

					var manDay = "";
					var text = matchArray[2].trim();
					var manDayMatch = /\b([0-9][0-9\.]*)(?=人日)/g.exec(matchArray[2]);
					if (manDayMatch) {
						manDay = manDayMatch[1];
						var regex = new RegExp('\\b' + manDay + '人日');
						text = matchArray[2].replace(regex, "").trim();
					}

					// よりシンプルな形で#を取り除く方法ってどんなものか
					return {
						'text': text,
						'project_name': /^#(.*)/g.exec(matchArray[1] || '#')[1],
						'man_day': manDay
					};
				};
			}],
			controllerAs: 'diffCost'
		};
	});
})();
//# sourceMappingURL=entry-form.js.map