/*!
* Data Aquarium Framework - Core
* Copyright 2008-2018 Code On Time LLC; Licensed MIT; http://codeontime.com/license
*/
(function () {
    Type.registerNamespace("Web");

    var _window = window,
        $window = $(_window),
        _web = _window.Web,
        _serializer = Sys.Serialization.JavaScriptSerializer,
        _jsExpRegex = /\{([\s\S]+?)\}/,
        dateTimeFormat = Sys.CultureInfo.CurrentCulture.dateTimeFormat,
        resources = _web.DataViewResources,
        resourcesData = Web.DataViewResources.Data,
        resourcesDataFilters = resourcesData.Filters,
        resourcesDataFiltersLabels = resourcesDataFilters.Labels,
        resourcesHeaderFilter = resources.HeaderFilter,
        resourcesModalPopup = resources.ModalPopup,
        resourcesPager = resources.Pager,
        resourcesMobile = resources.Mobile,
        resourcesFiles = resourcesMobile.Files,
        resourcesValidator = resources.Validator,
        resourcesActionsScopes = resources.Actions.Scopes,
        resourcesWhenLastCommandBatchEdit = resourcesActionsScopes.Form.Update.WhenLastCommandName.BatchEdit,
        fieldPropertiesWithTrueDefault = ['AllowQBE', 'AllowSorting', 'FormatOnClient', 'HtmlEncode'],
        fieldPropertiesWithZeroDefault = ['Len', 'Rows', 'Columns', 'Search', 'ItemsPageSize', 'Aggregate', 'OnDemandStyle', 'TextMode', 'MaskType', 'AutoCompletePrefixLength', 'CategoryIndex'],
        getPagePropertiesWithEmptyArrayDefault = ['Fields', 'Views', 'Categories', 'ActionGroups', 'Filter'],
        findDataView,
        _touch;

    function busy(isBusy) {
        if (_touch)
            _touch.busy(isBusy);
    }

    function configureDefaultProperties(f) {
        if (!f.Type)
            f.Type = 'String';
        if (f.Type == 'DataView' && !_touch)
            f.Hidden = true;
        // provide default values removed during compression
        configureDefaultValues(f, fieldPropertiesWithTrueDefault, true);
        configureDefaultValues(f, fieldPropertiesWithZeroDefault, 0);

        //if (isNullOrEmpty(field.HeaderText)) field.HeaderText = field.Label;
        //if (isNullOrEmpty(field.HeaderText)) field.HeaderText = field.Name;
        f.HeaderText = f.HeaderText || f.Label || f.Name;
        var tag = f.Tag,
            tagMatch = tag && tag.match(/survey-(form|data)+/);
        if (tagMatch)
            f.Tag += ' input-type-' + tagMatch[0].replace('-', '');

        if (f.Items == null)
            f.Items = [];
    }

    function configureDefaultValues(obj, properties, defaultValue) {
        var i, propName;
        for (i = 0; i < properties.length; i++) {
            propName = properties[i];
            if (obj[propName] == null)
                obj[propName] = defaultValue;
        }
    }

    function iterateMenuItems(callback, items) {
        if (!items) {
            var itemObj = Web.Menu.Nodes,
                propName;
            for (propName in itemObj)
                items = itemObj[propName];
        }
        $(items).each(function () {
            var item = this;
            if (callback.apply(item) == false)
                return false;
            if (item.children)
                if (!iterateMenuItems(callback, item.children))
                    return false;
        });
        return true;
    }

    function hasAccessToMembership() {
        var result = false;
        iterateMenuItems(function () {
            if (this.url && this.url.match(/\/membership(\.aspx)?$/i)) {
                result = true;
                return false;
            }
        });
        return result;
    }

    function closeHoverMonitorInstance() {
        var monitor = _web.HoverMonitor,
            instance = monitor && monitor._instance;
        if (instance)
            instance.close();
    }

    function parseInteger(s) {
        return parseInt(s, 10);
    }

    _web.DataViewMode = { View: 'View', Lookup: 'Lookup' };
    _web.DynamicExpressionScope = { Field: 0, ViewRowStyle: 1, CategoryVisibility: 2, DataFieldVisibility: 3, DefaultValues: 4, ReadOnly: 5, Rule: 6 };
    _web.AutoHideMode = { Nothing: 0, Self: 1, Container: 2 };
    _web.DynamicExpressionType = { RegularExpression: 0, ClientScript: 1, ServerExpression: 2, CSharp: 3, VisualBasic: 4, Any: 4 };
    _web.DataViewAggregates = ['None', 'Sum', 'Count', 'Avg', 'Max', 'Min'];
    _web.FieldSearchMode = { Default: 0, Required: 1, Suggested: 2, Allowed: 3, Forbidden: 4 }

    _web.PageState = {}
    _web.PageState._init = function () {
        var that = this,
            ostate,
            state;
        if (!that._state) {
            ostate = that._ostate = $('#__COTSTATE');
            state = ostate.val();
            that._state = state ? _serializer.deserialize(state) : {};
        }
    }
    _web.PageState._save = function () {
        if (this._ostate.length)
            this._ostate.val(_serializer.serialize(this._state));
    }
    _web.PageState.read = function (name) {
        this._init();
        return this._state[name];
    }
    _web.PageState.write = function (name, value) {
        this._init();
        this._state[name] = value;
        this._save();
    }

    Sys.StringBuilder.prototype.appendFormat = function (fmt, args) {
        this.append(String._toFormattedString(false, arguments));
    }

    var isNullOrEmpty = String.isNullOrEmpty = function (s) {
        return s == null || s.length == 0;
    }

    var isBlank = String.isBlank = function (s) {
        return s == null || typeof (s) == 'string' && s.match(_app._blankRegex) != null;
    }

    String._wordTrimRegex = /(\S{16})\S+/g;
    String._tagRegex = /<\/?\w.*?>/g;

    String.trimLongWords = function (s, maxLength) {
        if (s == null)
            return s;
        var re = this._wordTrimRegex;
        if (maxLength != null)
            re = new RegExp(String.format('(\\S{{{0}}})\\S+', maxLength), 'g');
        var result = s.replace(re, '$1...');
        if (s.match(String._tagRegex)) {
            var sb = new Sys.StringBuilder();
            var iterator = String._tagRegex;
            var m = iterator.exec(s);
            var lastIndex = 0;
            while (m) {
                tag = m[0];
                if (m.index > 0)
                    sb.append(s.substring(lastIndex, m.index).replace(re, '$1...'));
                lastIndex = m.index + tag.length;
                sb.append(tag);
                m = iterator.exec(s);
            }
            if (lastIndex < s.length)
                sb.append(s.substring(lastIndex));
            result = sb.toString();

        }
        return result;
    }

    String.htmlEncode = function (s) {
        return typeof s == 'string' && s.match(_app._htmlTest) ? s : _app.htmlEncode(s);
    }

    String.isJavaScriptNull = function (s) {
        return s == '%js%null' || s == 'null';
    };

    String.jsNull = '%js%null';

    _web.DataView = function (element) {
        var that = this;
        _app.initializeBase(that, [element]);
        that._controller = null;
        that._viewId = null;
        that._servicePath = null;
        that._baseUrl = null;
        that._pageIndex = -1;
        that._pageSize = resourcesPager.PageSizes[0];
        that._sortExpression = null;
        that._filter = [];
        that._externalFilter = [];
        that._categories = null;
        that._fields = null;
        that._allFields = null;
        that._rows = null;
        that._totalRowCount = 0;
        that._firstPageButtonIndex = 0;
        that._pageCount = 0;
        that._views = [];
        that._actionGroups = [];
        that._selectedKey = [];
        that._selectedKeyFilter = []
        that._lastCommandName = null;
        that._lastViewId = null;
        that._lookupField = null;
        that._filterFields = null;
        that._filterSource = null;
        that._mode = Web.DataViewMode.View;
        that._lookupPostBackExpression = null;
        that._domFilterSource = null;
        that._selectedKeyList = [];
        that._pageSizes = resourcesPager.PageSizes;
    }

    var _app = Web.DataView;
    _app.cache = {};
    _app.controllerSyncMap = {};
    _window.$app = _window.$appfactory = _app;

    RegExp.escape = function (s) {
        return s.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    };

    _app._blankRegex = /^\s*$/;
    _app._fieldMapRegex = /(\w+)\s*=\s*(\w+)/g;
    _app._fieldFilterRegex = /([\w\.\,]+):([\s\S]*)/;
    _app._filterRegex = /(\*|~|\$\w+\$|=|~|>=?|<(=|>){0,1})([\s\S]*?)(\0|$)$/;
    _app._filterIteratorRegex = /(\*|~|\$\w+\$|=|~|>=?|<(=|>){0,1})([\s\S]*?)(\0|$)/g;
    _app._keepCapitalization = /^\$(month|quarter|true|false)/;
    _app._listRegex = /\$and\$|\$or\$/;
    _app._simpleListRegex = /\s*,\s*/;
    _app._htmlTest = /<\w+.*?>/;
    _app.serializer = _serializer;

    if (!Array.isArray)
        Array.isArray = function (arg) {
            return Object.prototype.toString.call(arg) === '[object Array]';
        };

    _app.cssToIcon = function (cssClass) {
        var icon = cssClass && cssClass.match(/glyphicon-([\w\-]+)/);
        if (icon)
            return 'glyphicon ' + icon[0];
        return cssClass;
    }

    _app.toHexString = function (v) {
        if (v) {
            var hexValue = ['0x'];
            if (!Array.isArray(v))
                v = v.toString().split(_app._simpleListRegex);
            $(v).each(function () {
                hexValue.push(parseInt(this).toString(16));
            });

            v = hexValue.join('');
        }
        return v;
    }

    _app.online = function () { return navigator.onLine == true; }

    _app.htmlEncode = function (s) { if (s != null && typeof (s) != 'string') s = s.toString(); return s ? s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;') : s; }

    _app.toAjaxData = function (data) {
        return _app.htmlEncode(JSON ? JSON.stringify(data) : _serializer.serialize(data));
    }

    _app.htmlAttributeEncode = function (s) { return s != null && typeof (s) == 'string' ? s.replace(/\x27/g, '&#39;').replace(/\x22/g, '&quot;') : s; }

    _app.isIE6 = Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7;

    _app.prototype = {
        get_controller: function () {
            return this._controller;
        },
        set_controller: function (value) {
            this._controller = value;
        },
        useCase: function (name) {
            return this.get_useCase() == name;
        },
        get_useCase: function () {
            return this._useCase;
        },
        set_useCase: function (value) {
            this._useCase = value;
        },
        get_survey: function () {
            return this._survey;
        },
        set_survey: function (value) {
            this._survey = value;
        },
        get_viewId: function () {
            if (!this._viewId && this._views.length > 0)
                this._viewId = this._views[0].Id;
            var viewId = this._viewId;
            if (!viewId)
                viewId = '';
            return viewId;
        },
        set_viewId: function (value) {
            this._viewId = value;
        },
        get_newViewId: function () {
            return this._newViewId;
        },
        set_newViewId: function (value) {
            this._newViewId = value;
        },
        get_servicePath: function () {
            return this._servicePath || (typeof __servicePath == 'string' ? __servicePath : '');
        },
        set_servicePath: function set_servicePath(value) {
            this._servicePath = this.resolveClientUrl(value);
            if (!_app._servicePath) _app._servicePath = value;
        },
        get_baseUrl: function () {
            return this._baseUrl || (typeof __baseUrl == 'string' ? __baseUrl : '');
        },
        set_baseUrl: function (value) {
            if (value == '~') value = '/';
            this._baseUrl = value;
            if (!_app._baseUrl) _app._baseUrl = value;
        },
        get_siteUrl: function () {
            var servicePath = this.get_servicePath();
            var m = servicePath.match(/(^.+?\/)\w+\/\w+\.asmx/);
            if (!m)
                m = servicePath.match(/(^.+?\/)appservices\/_invoke$/);
            return m ? m[1] : '';
        },
        resolveClientUrl: function (url) {
            return url ? url.replace(/~\x2f/g, this.get_baseUrl()) : null;
        },
        get_hideExternalFilterFields: function () {
            return this._hideExternalFilterFields != false;
        },
        set_hideExternalFilterFields: function (value) {
            this._hideExternalFilterFields = value;
        },
        get_backOnCancel: function () {
            return this._backOnCancel == true;
        },
        set_backOnCancel: function (value) {
            this._backOnCancel = value;
        },
        get_confirmContext: function () {
            return this._confirmContext;
        },
        set_confirmContext: function (value) {
            this._confirmContext = value;
        },
        get_startPage: function () {
            return this._startPage;
        },
        set_startPage: function (value) {
            this._startPage = value;
        },
        get_startCommandName: function () {
            return this._startCommandName;
        },
        set_startCommandName: function (value) {
            this._startCommandName = value;
        },
        get_startCommandArgument: function () {
            return this._startCommandArgument;
        },
        set_startCommandArgument: function (value) {
            this._startCommandArgument = value;
        },
        get_exitModalStateCommands: function () {
            return this._exitModalStateCommands;
        },
        set_exitModalStateCommands: function (value) {
            this._exitModalStateCommands = value;
        },
        get_showActionBar: function () {
            var extension = this.extension();
            if (extension) {
                var options = extension.options();
                if (options.actionBar != null)
                    return options.actionBar == true;
            }
            return this._showActionBar != false;
        },
        set_showActionBar: function (value) {
            this._showActionBar = value;
        },
        get_showActionButtons: function () {
            var buttonLocation = this._showActionButtons;
            if (!buttonLocation) {
                buttonLocation = _touch ? 'Auto' : 'TopAndBottom';
                this._showActionButtons = buttonLocation;
            }
            return buttonLocation;
        },
        set_showActionButtons: function (value) {
            this._showActionButtons = value;
        },
        get_showSearchBar: function () {
            return this._showSearchBar == true && !(__tf != 4);
        },
        set_showSearchBar: function (value) {
            this._showSearchBar = value;
        },
        get_searchOnStart: function () {
            return this._searchOnStart == true;
        },
        set_searchOnStart: function (value) {
            this._searchOnStart = value;
            if (value) this.set_searchBarIsVisible(true);
        },
        get_searchBarIsVisible: function () {
            return this._searchBarIsVisible == true;
        },
        set_searchBarIsVisible: function (value) {
            this._searchBarIsVisible = value;
        },
        get_showModalForms: function () {
            if (_touch)
                return true;
            return this._showModalForms == true;
        },
        set_showModalForms: function (value) {
            this._showModalForms = value;
        },
        get_showDescription: function () {
            var extension = this.extension();
            if (extension) {
                var options = extension.options();
                return options.description == true;
            }
            return this._showDescription != false;
        },
        set_showDescription: function (value) {
            this._showDescription = value;
        },
        get_showFirstLetters: function () {
            return this._showFirstLetters == true;
        },
        set_showFirstLetters: function (value) {
            this._showFirstLetters = value;
        },
        get_autoSelectFirstRow: function () {
            return this._autoSelectFirstRow == true;
        },
        set_autoSelectFirstRow: function (value) {
            this._autoSelectFirstRow = value;
        },
        get_autoHighlightFirstRow: function () {
            return this._autoHighlightFirstRow == true;
        },
        set_autoHighlightFirstRow: function (value) {
            this._autoHighlightFirstRow = value;
        },
        get_refreshInterval: function () {
            return this._refreshInterval;
        },
        set_refreshInterval: function (value) {
            this._refreshInterval = !(__tf != 4) ? value : 0;
        },
        get_showViewSelector: function () {
            return this._showViewSelector != false;
        },
        set_showViewSelector: function (value) {
            this._showViewSelector = value;
        },
        get_showPager: function () {
            var pagerLocation = this._showPager;
            if (pagerLocation == null || pagerLocation == true) {
                pagerLocation = 'Bottom';
                this._showPager = pagerLocation;
            }
            else if (pagerLocation == false)
                pagerLocation = 'None';
            return pagerLocation;
        },
        set_showPager: function (value) {
            this._showPager = value;
        },
        get_showPageSize: function () {
            return this._showPageSize != false;
        },
        set_showPageSize: function (value) {
            this._showPageSize = value;
        },
        get_selectionMode: function () {
            return this._selectionMode;
        },
        set_selectionMode: function (value) {
            this._selectionMode = value;
        },
        multiSelect: function (value) {
            var that = this,
                result = that._multiSelect,
                selectionMode;
            if (arguments.length) {
                that._multiSelect = value;
                if (_touch)
                    that.pageProp('selectionMode', value ? 'Multiple' : 'Single');
            }
            else {
                if (result == null) {
                    selectionMode = _touch && that.pageProp('selectionMode');
                    result = that._multiSelect = (selectionMode || that._selectionMode) == 'Multiple';
                }
                return result;
            }
        },
        //get_cookie: function () {
        //    if (!this._cookie)
        //        this._cookie = this._id + 'cookie';
        //    return this._cookie;
        //},
        get_pagerButtonCount: function (refresh) {
            if (!this._pagerButtonCount || refresh) {
                var buttonCount = resourcesPager.PageButtonCount;
                if (this.get_pageCount() > buttonCount) {
                    buttonCount = buttonCount - 3;
                    if (buttonCount < 5)
                        buttonCount = 5;
                }
                this._pagerButtonCount = buttonCount;
            }
            return this._pagerButtonCount;
        },
        get_pageIndex: function () {
            return this._pageIndex;
        },
        set_pageIndex: function (value) {
            this._pageIndex = value;
            if (value >= 0) {
                var buttonCount = this.get_pagerButtonCount();
                if (value >= this._firstPageButtonIndex + buttonCount)
                    this._firstPageButtonIndex = value;
                else if (value < this._firstPageButtonIndex) {
                    this._firstPageButtonIndex -= buttonCount;
                    if (value < this._firstPageButtonIndex)
                        this._firstPageButtonIndex = value;
                }
                if (this._firstPageButtonIndex < 0)
                    this._firstPageButtonIndex = 0;
                if (this.get_pageCount() - this._firstPageButtonIndex < buttonCount)
                    this._firstPageButtonIndex = this.get_pageCount() - buttonCount;
                if (this._firstPageButtonIndex < 0)
                    this._firstPageButtonIndex = 0;
            }
            if (value == -2)
                this._pageOffset = 0;
        },
        get_pageOffset: function () {
            if (!this.get_isDataSheet())
                return 0;
            return this._pageOffset == null ? 0 : this._pageOffset;
        },
        set_pageOffset: function (value) {
            this._pageOffset = value;

        },
        get_categoryTabIndex: function () {
            return this._categoryTabIndex;
        },
        set_categoryTabIndex: function (value) {
            if (value != this._categoryTabIndex) {
                this._categoryTabIndex = value;
                if (!_touch) {
                    this._updateTabbedCategoryVisibility();
                    if (value != -1) {
                        if (this._modalPopup) {
                            this._resizeContainerBounds();
                            this._modalPopup.show();
                        }
                        _body_performResize();
                        if (this.editing()) {
                            this._focusedFieldName = null;
                            this._focus();
                        }
                    }
                }
            }
        },
        get_enabled: function () {
            return this._enabled == null ? true : this._enabled;
        },
        set_enabled: function (value) {
            this._enabled = value;
        },
        get_showInSummary: function () {
            return this._showInSummary;
        },
        set_showInSummary: function (value) {
            this._showInSummary = value;
        },
        get_summaryFieldCount: function () {
            return this._summaryFieldCount;
        },
        set_summaryFieldCount: function (value) {
            this._summaryFieldCount = value;
        },
        get_showLEVs: function () {
            return this._showLEVs;
        },
        set_showLEVs: function (value) {
            this._showLEVs = value;
        },
        get_tags: function () {
            return this._tag;
        },
        set_tags: function (value) {
            this._tag = value;
        },
        get_tag: function () {
            return this._tag;
        },
        set_tag: function (value) {
            this._tag = value;
        },
        get_pageSize: function () {
            return this._pageSize;
        },
        set_pageSize: function (value) {
            this._pagerSize = value;
            this._pageSize = value;
            this._pageOffset = 0;
            delete this._viewColumnSettings;
            if (Array.indexOf(this._pageSizes, value) == -1) {
                this._pageSizes = Array.clone(this._pageSizes);
                Array.insert(this._pageSizes, 0, value);
            }
            //        if (this._fields != null) {
            //            this.set_pageIndex(-2);
            //            this._loadPage();
            //        }
            if (this._fields != null)
                this.refreshData();
        },
        get_groupExpression: function () {
            return this._groupExpression;
        },
        set_groupExpression: function (value) {
            this._groupExpression = value;
            this._checkedSortGroup = false;
        },
        get_sortExpression: function () {
            var sort = this._sortExpression,
                sortBy,
                groupBy = this._groupExpression,
                i = 0,
                groupField, sortField;
            if (!this._checkedSortGroup && groupBy) {
                if (!sort)
                    sort = '';
                this._checkedSortGroup = true;
                groupBy = groupBy.trim().split(_app._simpleListRegex);
                sortBy = sort.trim().split(_app._simpleListRegex);
                while (i < groupBy.length) {
                    groupField = groupBy[i];
                    if (i < sortBy.length) {
                        sortField = sortBy[i].split(/\s+/);
                        if (groupField != sortField[0])
                            sortBy.splice(i, 0, groupField);
                    }
                    else
                        sortBy.splice(i, 0, groupField);
                    i++;
                }
                sort = sortBy.join(',');
                if (sort.match(/,$/))
                    sort = sort.substring(0, sort.length - 1);
                this._sortExpression = sort
            }
            return sort;
        },
        set_sortExpression: function (value) {
            this._checkedSortGroup = false;
            if (!value || value.length == 0)
                this._sortExpression = null;
            else {
                var expression = value.match(/^(\w+)\s*((asc|desc)|$)/i)
                if (expression && expression[2].length == 0)
                    if (isNullOrEmpty(this._sortExpression) || this._sortExpression.match(/^(\w+)\s*/)[1] != expression[1])
                        this._sortExpression = value + ' asc';
                    else if (this._sortExpression.endsWith(' asc'))
                        this._sortExpression = value + ' desc';
                    else
                        this._sortExpression = value + ' asc';
                else
                    this._sortExpression = value;
            }
        },
        set_id: function (value) {
            if (!this._id)
                this._id = value;
        },
        get_appId: function () {
            return this._id;
        },
        set_appId: function (value) {
            if (_touch)
                this._id = value;
        },
        get_appFilterSource: function () {
            return this._filterSource;
        },
        set_appFilterSource: function (value) {
            if (_touch)
                this._filterSource = value;
        },
        get_filterSource: function () {
            return this._filterSource;
        },
        set_filterSource: function (value) {
            if (!this._filterSource)
                this._filterSource = value;
        },
        get_filterFields: function () {
            return this._filterFields;
        },
        set_filterFields: function (value) {
            this._filterFields = value;
        },
        get_visibleWhen: function () {
            return this._visibleWhen;
        },
        set_visibleWhen: function (value) {
            this._visibleWhen = value;
        },
        get_showQuickFind: function () {
            return this._showQuickFind != false;
        },
        get_quickFindText: function () {
            return isNullOrEmpty(this._quickFindText) ? resources.Grid.QuickFindText : this._quickFindText;
        },
        set_quickFindText: function (value) {
            this._quickFindText = value;
        },
        get_quickFindElement: function () {
            return $get(this.get_id() + '_QuickFind');
        },
        set_showQuickFind: function (value) {
            this._showQuickFind = value;
        },
        get_showRowNumber: function () {
            return this._showRowNumber == true;
        },
        set_showRowNumber: function (value) {
            this._showRowNumber = value;
        },
        get_filter: function () {
            if (this.get_lookupField() == null && (this.get_pageIndex() == -1 && !this._allFields || this._externalFilter.length > 0 && this._filter.length == 0)) {
                if (this.get_domFilterSource()) {
                    this._externalFilter = [];
                    var fieldNames = this.get_filterFields().split(_app._simpleListRegex);
                    var fieldValues = this.get_domFilterSource().value.split(_app._simpleListRegex);
                    for (var i = 0; i < fieldNames.length; i++)
                        Array.add(this._externalFilter, { Name: fieldNames[i], Value: fieldValues[i] });
                }
                else {
                    var params = _app._commandLine.match(/\?([\s\S]+)/);
                    if (params && (this.get_filterSource() != 'None' && this.get_filterSource() == null) && !this.get_useCase()) {
                        this._externalFilter = [];
                        var iterator = /(\w+)=([\S\s]*?)(&|$)/g;
                        var m = iterator.exec(params[1]);
                        while (m) {
                            if (m[1] != 'ReturnUrl') Array.add(this._externalFilter, { Name: m[1], Value: m[2].length == 0 ? String.jsNull : decodeURIComponent(m[2]) });
                            m = iterator.exec(params[1]);
                        }
                    }
                }
                this.applyExternalFilter(this.get_isModal() || (_touch || this.useCase('$app')) && this._filter);
            }
            else if (this.get_filterSource() == 'Context' && this._externalFilter.length > 0)
                this.applyExternalFilter(true);
            if (this._startupFilter) {
                if (this.readContext('disableStartFilter') != true) {
                    Array.addRange(this._filter, this._startupFilter);
                    this.writeContext('disableStartFilter', true);
                }
                this._startupFilter = null;
            }
            return this._filter;
        },
        set_filter: function (value) {
            this._filter = value;
        },
        get_startupFilter: function () {
            return this._startupFilter;
        },
        set_startupFilter: function (value) {
            this._startupFilter = value;
        },
        get_externalFilter: function () {
            return this._externalFilter;
        },
        set_externalFilter: function (value) {
            this._externalFilter = value ? value : [];
        },
        get_ditto: function () {
            return this._ditto;
        },
        set_ditto: function (value) {
            this._ditto = value;
        },
        get_modalAnchor: function () {
            return this._modalAnchor;
        },
        set_modalAnchor: function (value) {
            this._modalAnchor = value;
        },
        get_description: function () {
            return this._description;
        },
        set_description: function (value) {
            this._description = value;
        },
        get_isModal: function () {
            return this._modalPopup != null || this._isModal;
        },
        get_categories: function () {
            return this._categories;
        },
        get_fields: function () {
            return this._fields;
        },
        get_rows: function () {
            return this._rows;
        },
        data: function (type) {
            var that = this;

            function filterToData(dataView) {
                var data = {},
                    matchCount = 0,
                    filter = (type == 'filter' ? dataView._filter : (type == 'combined' ? dataView.combinedFilter() : dataView._combinedFilter([])));
                if (filter)
                    filter.forEach(function (fd) {
                        var m = fd.match(/(.+?)\:(=|<>|>=|<=|>|<|(\$\w+\$))(.*?)\0?$/),
                            fieldName, field, operator, value;
                        if (m) {
                            fieldName = m[1];
                            operator = m[2];
                            value = m[4];
                            if (fieldName == '_match_' || fieldName == '_donotmatch_')
                                data['_Match' + (++matchCount).toString()] = (fieldName == '_match_' ? '$match' : '$donotmatch') + operator.substring(1, 4);
                            else {
                                field = dataView.findField(fieldName);
                                if (operator.match(/\$$/))
                                    operator = operator.substring(0, operator.length - 1);
                                if (matchCount > 0)
                                    fieldName = '_Match' + matchCount.toString() + '_' + fieldName;
                                if (operator.match(/^\$(month|quarter)\d+$/i)) {
                                    data[fieldName + '_op2'] = operator;
                                    data[fieldName + '_op'] = 'all-dates-in-period';
                                }
                                else
                                    data[fieldName + '_op'] = operator;
                                if (value) {
                                    value = value.split(/\$and\$|\$or\$/g);
                                    value.forEach(function (v, index) {
                                        value[index] = dataView.convertStringToFieldValue(field, v);
                                    });
                                    data[fieldName] = value[0];
                                    if (operator == '$between')
                                        data[fieldName + '_v2'] = value[1];
                                    else if (value.length > 1)
                                        data[fieldName] = value;
                                }
                            }
                        }
                        else if (fd.match(/^_quickfind_:~/))
                            data['QuickFind'] = JSON.parse(fd.substring(17));
                    });
                return data;
            }

            if (type == 'master') {
                var master = that.get_master();
                return master ? master.data() : null;
            }
            else if (type == 'context') {
                var parentDataView = that.get_parentDataView();
                return parentDataView ? parentDataView.data() : null;
            }
            else if (type && type.match(/^(filter|search|combined)$/))
                return filterToData(that);
            else
                return that.survey('data');
        },
        row: function () {
            return this.survey('row');
        },
        survey: function (method) {
            var dataView = this,
                row, values;
            if (method == 'row') {
                if (_touch)
                    row = dataView.editRow();
                else {
                    row = [];
                    values = dataView._collectFieldValues();
                    dataView._validateFieldValues(values, false);
                    $(values).each(function () {
                        var fv = this,
                            f = dataView.findField(fv.Name);
                        if (f)
                            row[f.Index] = fv.NewValue;
                    });
                }
                $(dataView._pendingUploads).each(function () {
                    var upload = this,
                        f = dataView.findField(upload.fieldName);
                    row[f.Index] = upload.files;
                });
                return row;
            }
            else if (method == 'data') {
                row = dataView.row();
                var obj = {},
                    allFields = dataView._allFields;
                $(allFields).each(function (index) {
                    var f = this,
                        name = f.Name;
                    if (!(name == 'sys_pk_' || name == 'Status' && index == allFields.length - 1))
                        obj[name] = row[f.Index];
                });
                return obj;
            }
            return dataView._survey;
        },
        changed: function () {
            var that = this, i, odp = that.odp,
                unchangedRow = that._unchangedRow,
                editRow = that._editRow,
                result;
            if (arguments.length > 0 && arguments[0] == 'ignore') {
                result = that.changed() || odp && odp.root(that) && odp.is(':dirty');
                if (arguments.length == 1)
                    return !result || (that.tagged('discard-changes-prompt-none') || that._ignoreUnsavedChanges);
                else
                    that._ignoreUnsavedChanges = arguments[1];
            }
            else
                if (that.editing())
                    if (that._pendingUploads && that._pendingUploads.length)
                        result = true;
                    else if (unchangedRow && that.editing())
                        for (i = 0; i < unchangedRow.length; i++) {
                            if (editRow[i] != unchangedRow[i]) {
                                result = true;
                                break;
                            }
                        }
            return result;
        },
        editRow: function () {
            var that = this,
                master,
                extension,
                filter,
                row = that._editRow,
                originalRow, masterRow;
            if (!row) {
                if (that.inserting()) {
                    row = (that._newRow || []).slice(0);
                    $(that._externalFilter).each(function () {
                        var fv = this,
                            field;
                        if (fv) {
                            field = that.findField(fv.name || fv.Name);
                            if (field && !(field.IsPrimaryKey && field.ReadOnly) && row[field.Index] == null)
                                row[field.Index] = fv.name ? fv.value : fv.Value;
                        }
                    });
                    master = that.get_master();
                    if (master) {
                        masterRow = master.editRow();
                        filter = that.get_externalFilter();

                        $(filter).each(function (index) {
                            var v = this,
                                useValue = true,
                                f = that.findField(v.Name),
                                mf;
                            if (f) {
                                if (f.ItemsDataValueField) {
                                    mf = master.findField(f.ItemsDataValueField);
                                    if (mf) {
                                        useValue = false;
                                        row[f.Index] = masterRow[mf.Index];
                                    }
                                }
                                if (f.Index != f.AliasIndex && f.ItemsDataTextField) {
                                    mf = master.findField(f.ItemsDataTextField);
                                    if (mf)
                                        row[f.AliasIndex] = masterRow[mf.Index];
                                }
                                if (useValue)
                                    row[f.Index] = that.convertStringToFieldValue(f, v.Value); //typeof v.Value == 'string' ? that.convertStringToFieldValue(f, v.Value) : v.Value;
                            }

                        });
                    }
                    originalRow = [];
                    $(row).each(function (index) {
                        originalRow[index] = null;
                    });
                }
                else {
                    extension = that.extension()
                    row = extension && extension.commandRow ? extension.commandRow() : [];
                    if (!row)
                        row = [];
                    originalRow = row.slice(0);
                }
                row = that._editRow = row.slice(0);
                that._originalRow = originalRow;
            }
            return row
        },
        headerField: function () {
            var dataView = this;
            headerField = dataView._headerField;
            if (!headerField)
                $(dataView._fields).each(function () {
                    var f = this;
                    if (!f.Hidden && !f.OnDemand) {
                        headerField = f;
                        return false;
                    }
                });
            headerField = dataView._allFields[headerField.AliasIndex];
            return headerField;
        },
        context: function (options) {
            var that = this,
                dv = that._filterSource ? findDataView(that._filterSource) : that.get_parentDataView(that),
                row, obj = {};
            if (arguments.length) {
                if (dv.get_isForm()) {
                    if (dv._survey)
                        return dv.survey(options);
                    row = dv.editRow();
                }
                else
                    row = dv.get_selectedRow();
                if (options == 'row')
                    return row;
                $(dv._allFields).each(function () {
                    var f = this;
                    obj[f.Name] = row[f.Index];
                });
                return obj;
            }
            else
                return dv;
        },
        originalRow: function () {
            return this._originalRow;
        },
        get_selectedRow: function () {
            var extension = this.extension();
            if (extension)
                return _touch ? extension.commandRow() : this._selectedRow;
            return this._rows ? this._rows[this._selectedRowIndex != null ? this._selectedRowIndex : 0] : [];
        },
        get_pageCount: function () {
            return this._pageCount;
        },
        get_aggregates: function () {
            return this._aggregates;
        },
        get_views: function () {
            return this._views;
        },
        actionGroups: function (scopeList) {
            var that = this,
                result = [],
                groupList = that._actionGroups,
                i, j, k,
                group, action, newGroup,
                scope;
            if (typeof scopeList == 'string')
                scopeList = [scopeList];
            for (k = 0; k < scopeList.length; k++) {
                scope = scopeList[k];
                for (i = 0; i < groupList.length; i++) {
                    group = groupList[i];
                    newGroup = null;
                    if (group.Scope == scope || group.Id == scope) {
                        for (j = 0; j < group.Actions.length; j++) {
                            action = group.Actions[j];
                            if (that._isActionAvailable(action)) {
                                if (!newGroup)
                                    result.push(newGroup = { Scope: group.Scope, HeaderText: group.HeaderText, Flat: group.Flat, Id: group.Id, Actions: [] });
                                newGroup.Actions.push(action);
                            }
                        }
                    }
                }
            }
            return result;
        },
        actions: function (scope) {
            var groups = this.actionGroups(scope);
            return groups.length ? groups[0].Actions : null;
        },
        get_actionGroups: function (scope, all) {
            var groups = [];
            for (var i = 0; i < this._actionGroups.length; i++) {
                if (this._actionGroups[i].Scope == scope) {
                    var group = this._actionGroups[i];
                    var current = all ? group : null;
                    if (!all) {
                        for (var j = 0; j < group.Actions.length; j++) {
                            if (this._isActionAvailable(group.Actions[j])) {
                                current = this._actionGroups[i]
                                break;
                            }
                        }
                    }
                    if (current) Array.add(groups, current);
                }
            }
            return groups;
        },
        get_actions: function (scope, all) {
            var result = [];
            for (var i = 0; i < this._actionGroups.length; i++)
                if (this._actionGroups[i].Scope == scope) {
                    var ag = this._actionGroups[i];
                    if (all)
                        Array.addRange(result, ag.Actions);
                    else
                        return ag.Actions;
                }
            return result;
        },
        get_action: function (path) {
            var id = path.split(/\//);
            for (var i = 0; i < this._actionGroups.length; i++) {
                var ag = this._actionGroups[i];
                if (ag.Id == id[0])
                    for (var j = 0; j < ag.Actions.length; j++) {
                        var a = ag.Actions[j];
                        if (a.Id == id[1])
                            return a;
                    }
            }
            return null;
        },
        get_selectedKey: function () {
            return this._selectedKey;
        },
        set_selectedKey: function (value) {
            this._selectedKey = value;
        },
        get_selectedKeyFilter: function () {
            return this._selectedKeyFilter;
        },
        set_selectedKeyFilter: function (value) {
            this._selectedKeyFilter = value;
        },
        _get_selectedValueElement: function () {
            var result = this._selectedValueElement;
            if (!result) {
                result = this._selectedValueElement = $(String.format('#{0}_{1}_SelectedValue', this.get_id(), this.get_controller()));
                //return $get(String.format('{0}$SelectedValue', this.get_id()));
            }
            return result.length ? result[0] : null;
        },
        get_selectedValue: function () {
            var selectedValue = this.readContext('SelectedValue');
            if (selectedValue)
                return selectedValue;
            var sv = this._get_selectedValueElement();
            return sv ? sv.value : '';
        },
        set_selectedValue: function (value) {
            if (this._hasSearchAction) return;
            this.writeContext('SelectedValue', value.toString());
            var selectedValue = this._get_selectedValueElement();
            if (selectedValue)
                selectedValue.value = value != null ? value : '';
        },
        get_keyRef: function () {
            var key = this.get_selectedKey();
            if (!key) return null;
            var ref = '';
            for (var i = 0; i < this._keyFields.length; i++) {
                if (i > 0) ref += '&';
                ref = String.format('{0}{1}={2}', ref, this._keyFields[i].Name, key[i]);
            }
            return ref;
        },
        get_showIcons: function () {
            return this._icons != null && this._lookupField == null || this.get_showRowNumber();
        },
        get_showMultipleSelection: function () {
            return this.multiSelect() && this._hasKey();
        },
        get_sysColCount: function () {
            var count = 0;
            if (this.get_showIcons())
                count++;
            if (this.get_showMultipleSelection())
                count++;
            if (this.get_isDataSheet())
                count++;
            return count;
        },
        _createRowKey: function (index) {
            var r = typeof index == 'number' ? this._rows[index] : arguments[0],
                v = '',
                i, f, kv;
            for (i = 0; i < this._keyFields.length; i++) {
                f = this._keyFields[i];
                if (v.length > 0) v += ',';
                kv = r[f.Index];
                v += kv == null ? 'null' : kv.toString()
            }
            return v;
        },
        batchEdit: function (commandArgument, result) {
            var that = this,
                view = commandArgument || that._viewId;
            if (!result)
                _app.execute({
                    controller: that._controller,
                    view: view,
                    requiresMetadata: true,
                    requiresData: false
                    //,
                    //success2: function (result) {
                    //    that.batchEdit(view, result);
                    //}
                }).done(function (result) {
                    that.batchEdit(view, result);
                });
            else {
                var batchEditNotAllowed = [],
                    batchEditAllowed = [],
                    editFields = [],
                    fieldQuestions = [
                    ],
                    survey = {
                        text: that.get_view().Label,
                        text2: resourcesActionsScopes.Grid.BatchEdit.HeaderText,// + ' (' + that._selectedKeyList.length + ')',
                        parent: that._id,
                        controller: that._id + '_' + view + '_BatchEdit',
                        context: { controller: that._controller, view: view },
                        topics: [
                            {
                                description: String.format(resourcesMobile.ItemsSelectedMany, that._selectedKeyList.length) + '. ' + String.format(resources.Views.DefaultCategoryDescriptions.$DefaultEditDescription, that.get_view().Label),
                                questions: fieldQuestions
                            }
                        ],
                        options: {
                            discardChangesPrompt: false,
                            materialIcon: 'edit'
                        },
                        submit: 'batcheditsubmit.dataview.app',
                        submitText: resourcesWhenLastCommandBatchEdit.HeaderText
                    };
                // enumerate batch-editable fields
                $(result.fields).each(function () {
                    var f = this,
                        tag = f.Tag;
                    if (!f.Hidden && !_field_isReadOnly.call(f) && !f.OnDemand && f.Type != 'DataView') {
                        editFields.push(f);
                        if (tag)
                            if (tag.match(/\bbatch\-edit\-disabled\b/))
                                batchEditNotAllowed.push(f);
                            else if (tag.match(/\bbatch\-edit\b/))
                                batchEditAllowed.push(f);
                    }
                });
                if (batchEditAllowed.length)
                    editFields = batchEditAllowed;
                else if (batchEditNotAllowed.length) {
                    var i = 0;
                    while (i < editFields.length)
                        if (batchEditNotAllowed.indexOf(editFields[i]) != -1)
                            editFields.splice(i, 1);
                        else
                            i++;
                }
                // continue with the configuration of the survey
                if (editFields.length) {
                    // build out a cascading dependency map
                    var fieldMap = {},
                        dependencyMap = {};

                    $(editFields).each(function () {
                        var f = this;
                        fieldMap[f.Name] = f;
                    });

                    function enumerateCascadeParents(f, list) {
                        var context = f.ContextFields,
                            m, matches = [], f2;
                        if (context) {
                            while (m = _app._fieldMapRegex.exec(context))
                                matches.push(m);
                            $(matches).each(function () {
                                var m = this;
                                f2 = fieldMap[m[2]];
                                if (f2 && !f2._depends) {
                                    f2._depends = true;
                                    list.push(f2);
                                    enumerateCascadeParents(f2, list);
                                }
                            });
                        }
                    }

                    $(editFields.slice(0).reverse()).each(function () {
                        var f = this,
                            list;
                        if (f.ContextFields && !f._depends) {
                            list = dependencyMap[f.Name] = [];
                            enumerateCascadeParents(f, list);
                            list = list.reverse();
                        }
                    });
                    //var row = that.row();
                    // create questions for each field
                    $(editFields).each(function () {
                        var f = this,
                            //targetField = that.findField(f.Name),
                            allowNulls = f.AllowNulls == true,
                            qcb = { name: f.Name + '_BatchEdit', type: 'bool', value: false, required: !_touch, text: f.HeaderText || f.Label, items: { style: 'CheckBox' }, options: { lookup: { autoAdvance: true } } },
                            q = {
                                name: f.Name, type: f.Type, len: f.Len, placeholder: allowNulls ? resourcesValidator.Optional : resourcesValidator.Required, text: false, columns: f.Columns, rows: f.Rows,
                                visibleWhen: '$row.' + f.Name + '_BatchEdit == true', extended: { allowNulls: allowNulls },
                                format: f.DataFormatString, context: f.ContextFields, options: { clearOnHide: true }
                            },
                            itemsStyle = f.ItemsStyle;
                        if (itemsStyle) {
                            var items = {};
                            items.style = itemsStyle;
                            if (f.ItemsDataController) {
                                items.controller = f.ItemsDataController;
                                items.view = f.ItemsDataView;
                                items.dataValueField = f.ItemsDataValueField;
                                items.dataTextField = f.ItemsDataTextField;
                                items.newView = f.ItemsNewDataView;
                                items.targetController = f.ItemsTargetController;
                            }
                            else {
                                items.list = [];
                                $(f.Items).each(function () {
                                    var item = this;
                                    items.list.push({ value: item[0], text: item[1] });
                                });
                            }
                            q.items = items;
                            if (f.AliasName)
                                $(result.fields).each(function () {
                                    var af = this;
                                    if (af.Name == f.AliasName) {
                                        qcb.text = af.Label || af.HeaderText;
                                        q.altText = qcb.text;
                                        return false;
                                    }
                                });
                        }
                        if (itemsStyle || f.Type == 'Boolean')
                            q.options.lookup = {
                                nullValue: false
                            }
                        if (f._depends) {
                            q.placeholder = qcb.text;
                            f._question = q;
                        }
                        else
                            fieldQuestions.push(qcb, q);
                        //if (targetField)
                        //    q.value = row[targetField.Index];
                    });
                    // extend fields with cascading depedencies
                    var i = 1;
                    while (i < fieldQuestions.length) {
                        var q = fieldQuestions[i],
                            dependency = dependencyMap[q.name];
                        if (dependency) {
                            q.placeholder = fieldQuestions[i - 1].text;
                            q.text = false;
                            q.extended.dependency = dependency;
                            $(dependency).each(function () {
                                var q2 = this._question;
                                q2.visibleWhen = q.visibleWhen;
                                q2.text = '';
                                fieldQuestions.splice(i++, 0, q2);
                            });
                        }
                        i += 2;
                    }
                    // show the batch edit survey
                    _app.survey(survey);
                }
            }
        },
        toggleSelectedRow: function (index) {
            var startIndex = index != null ? index : 0,
                endIndex = index != null ? index : this._rows.length - 1,
                btn = $get(this.get_id() + '_ToggleButton'),
                i, j, key, checked, cb, elem, si;
            for (i = startIndex; i <= endIndex; i++) {
                key = this._createRowKey(i);
                j = Array.indexOf(this._selectedKeyList, key);
                if (j != -1)
                    Array.removeAt(this._selectedKeyList, j);
                checked = index == null ? btn.checked : j == -1;
                if (checked)
                    Array.add(this._selectedKeyList, key);
                cb = $get(this.get_id() + '_CheckBox' + i);
                if (cb) {
                    cb.checked = checked;
                    if (checked)
                        $(cb).addClass('Selected');
                    else
                        $(cb).removeClass('Selected');
                    elem = cb;
                    while (elem && elem.tagName != 'TR')
                        elem = elem.parentNode;
                    if (checked)
                        $(elem).addClass('MultiSelectedRow');
                    else
                        $(elem).removeClass('MultiSelectedRow');
                }
            }
            this._skipCellFocus = this.get_isDataSheet();
            this.set_selectedValue(this._selectedKeyList.join(';'));
            si = $get(this.get_id() + '$SelectionInfo');
            if (si) si.innerHTML = this._selectedKeyList.length == 0 ? '' : String.format(resourcesPager.SelectionInfo, this._selectedKeyList.length);
            if (index >= 0)
                this.executeRowCommand(index, 'Select')
            else
                this.delayedRefresh();
        },
        delayedRefresh: function (refresh, delay) {
            if (!delay)
                delay = 1000;
            if (refresh) {
                delete this._delayedRefreshTimer;
                if (!(this._isBusy || _app._navigated || Sys.Application._disposing))
                    this.refresh(true);
            }
            else {
                if (this._delayedRefreshTimer)
                    clearTimeout(this._delayedRefreshTimer);
                var self = this;
                this._delayedRefreshTimer = setTimeout(function () {
                    self.delayedRefresh(true);
                }, delay);
            }
        },
        get_view: function (id) {
            if (!id) id = this.get_viewId();
            if (!this._view || this._view.Id != id) {
                for (var i = 0; i < this._views.length; i++)
                    if (this._views[i].Id == id) {
                        this._view = this._views[i];
                        break;
                    }
            }
            return this._view;
        },
        get_viewType: function (id) {
            var view = this.get_view(id);
            if (this._viewTypes) {
                var t = this._viewTypes[view ? view.Id : id];
                if (t != null)
                    return t;
            }
            return view ? view.Type : null;
        },
        get_isGrid: function (id) {
            var type = this.get_viewType(id);
            return type == 'Grid' || type == 'DataSheet'/* || type == 'Tree'*/;
        },
        get_isForm: function (id) {
            var type = this.get_viewType(id);
            return type == 'Form';
        },
        get_isDataSheet: function (id) {
            var type = this.get_viewType(id);
            if (__tf != 4) return false;
            if (this._viewTypes) {
                var t = this._viewTypes[this.get_viewId()];
                if (t != null)
                    type = t;
            }
            return type == 'DataSheet';
        },
        /*get_isTree: function (id) {
        var type = this.get_viewType(id);
        return type == 'Tree' && __tf == 4;
        },*/
        get_isChart: function () {
            return this.get_viewType() == 'Chart';
        },
        get_lastViewId: function () {
            return this._lastViewId;
        },
        set_lastViewId: function (value) {
            this._lastViewId = value;
        },
        get_lastCommandName: function () {
            return this._lastCommandName;
        },
        set_lastCommandName: function (value) {
            this._lastCommandName = value;
            this._lastCommandArgument = null;
            $closeHovers();
        },
        get_lastCommandArgument: function () {
            return this._lastCommandArgument;
        },
        set_lastCommandArgument: function (value) {
            this._lastCommandArgument = value;
        },
        get_isEditing: function () {
            return this.editing();
        },
        editing: function () {
            var that = this,
                lastCommandName = that._lastCommandName,
                editing = that._editing;
            return editing == null && (lastCommandName == 'Edit' || lastCommandName == 'New' || /*lastCommandName == 'BatchEdit' || */lastCommandName == 'Duplicate') || editing == true;
        },
        get_isInserting: function () {
            return this.inserting();
        },
        inserting: function () {
            var lastCommandName = this._lastCommandName;
            return lastCommandName == 'New' || lastCommandName == 'Duplicate';
        },
        get_lookupField: function () {
            return this.get_mode() == Web.DataViewMode.View ? this._lookupField : this._fields[0];
        },
        set_lookupField: function (value) {
            this._lookupField = value;
        },
        get_lookupContext: function () {
            var that = this,
                lc = that._lookupContext,
                f = that._lookupInfo ? that._lookupInfo.field : that.get_lookupField();
            return lc ? lc : (f ? { 'FieldName': f.Name, 'Controller': f._dataView.get_controller(), 'View': f._dataView.get_viewId() } : null);
        },
        get_mode: function () {
            return this._mode;
        },
        set_mode: function (value) {
            this._mode = value;
        },
        get_lookupValue: function () {
            return this._lookupValue;
        },
        set_lookupValue: function (value) {
            this._lookupValue = value;
        },
        get_lookupText: function () {
            return this._lookupText;
        },
        set_lookupText: function (value) {
            this._lookupText = value;
        },
        get_lookupPostBackExpression: function () {
            return this._lookupPostBackExpression;
        },
        set_lookupPostBackExpression: function (value) {
            this._lookupPostBackExpression = value;
        },
        get_domFilterSource: function () {
            return this._domFilterSource;
        },
        set_domFilterSource: function (value) {
            this._domFilterSource = value;
        },
        get_showDetailsInListMode: function () {
            return this._showDetailsInListMode != false;
        },
        set_showDetailsInListMode: function (value) {
            this._showDetailsInListMode = value;
        },
        get_autoHide: function () {
            return !isNullOrEmpty(this.get_visibleWhen()) && this._autoHide == Web.AutoHideMode.Nothing ?
                Web.AutoHideMode.Self :
                this._autoHide == null ? Web.AutoHideMode.Nothing : this._autoHide;
        },
        set_autoHide: function (value) {
            this._autoHide = value;
        },
        //get_transaction: function () {
        //    return this._transaction;
        //},
        initialize: function () {
            _app.callBaseMethod(this, 'initialize');
            this._bodyKeydownHandler = Function.createDelegate(this, this._bodyKeydown);
            this._filterSourceSelectedHandler = Function.createDelegate(this, this._filterSourceSelected);
            this._quickFindHandlers = {
                'focus': this._quickFind_focus,
                'blur': this._quickFind_blur,
                'keydown': this._quickFind_keydown
            }
            this._checkMaxLengthHandler = function (e) {
                var maxLen = parseInteger(this.getAttribute('maxlength'));
                if (this.value.length > maxLen) {
                    this.value = this.value.substr(0, maxLen);
                    e.preventDefault();
                    e.stopPropagation();
                }
            };
        },
        dispose: function () {
            var that = this,
                survey = that._survey;
            that._disposePendingUploads();
            if (!Sys.Application._disposing) {
                $(that._element).find('iframe').remove();
                that._detachBehaviors();
            }
            if (that._container || _touch) {
                var extension = this.extension();
                if (extension && extension.dispose)
                    extension.dispose();
            }
            that._wsRequest = null;
            that._stopInputListener();
            that._disposeModalPopup();
            that._disposeFieldFilter();
            that._disposeSearchBarExtenders();
            that._disposeImport();
            that._disposeFields();
            that._lookupField = null;
            that._parentDataView = null;
            that._bodyKeydownHandler = null;
            that._filterSourceSelectedHandler = null;
            that._restoreEmbeddedViews();
            delete that._container;
            that._cancelCallback = null;
            that._doneCallback = null;
            if (survey) {
                for (var key in survey)
                    if (typeof survey[key] === 'function')
                        survey[key] = null;
                that._survey = null;
            }
            clearTimeout(that._valueChangedTimeout);
            _app.callBaseMethod(that, 'dispose');
        },
        get_master: function () {
            var filterSource = this.get_filterSource();
            return filterSource ? _app.findDataView(filterSource) : null;
        },
        tag: function (tag) {
            if (tag) {
                this._tag = tag + ' ' + (this._tag || '').replace(/,/g, ' ').trim();
                this._tagList = null;
            }
        },
        untag: function (tag) {
            if (this._tag) {
                this.get_isTagged('');
                var tagList = this._tagList,
                    index = Array.indexOf(tagList, tag);
                if (index >= 0)
                    tagList.splice(index, 1);
                this._tag = tagList.join();
            }
            this._tagList = null;
        },
        get_isTagged: function (tag) {
            var that = this,
                tagList = that._tagList;
            if (!tagList) {
                tagList = that._tag;
                if (tagList)
                    tagList = that._tagList = tagList.replace(/,/g, ' ').split(/\s+/);
            }
            return tagList != null && Array.contains(tagList, tag);
        },
        tagged: function (tags) {
            var that = this,
                isRegExp = tags.ignoreCase != null;
            if (arguments.length == 1)
                return (isRegExp || tags.match(/\-$/)) ? (that._tag || '').match(isRegExp ? tags : typeof new RegExp(tags)) : that.get_isTagged(tags);
            for (var i = 0; i < tags.length; i++)
                if (that.get_isTagged(tags[i]))
                    return true;
            return false;
        },
        updated: function () {
            _app.callBaseMethod(this, 'updated');
            if (!this._controller) return;
            if (this.get_servicePath().startsWith('http'))
                this.set_baseUrl(this.get_siteUrl());
            var selectedValue = this.get_selectedValue();
            if (selectedValue.length > 0) {
                if (!_touch) {
                    this.set_autoHighlightFirstRow(false);
                    this.set_autoSelectFirstRow(false);
                }
                if (this.multiSelect())
                    this._selectedKeyList = selectedValue.split(';');
                else {
                    this._selectedKey = selectedValue.split(',');
                    this._pendingSelectedEvent = true;
                }
            }
            if (!this._container && !_touch) {
                this.get_element().innerHTML = '';
                this._container = document.createElement('div');
                this.get_element().appendChild(this._container);
                $(this._container).addClass('DataViewContainer');
                var elementId = this._element.id;
                var idMatch = elementId.split(/_/);
                Sys.UI.DomElement.addCssClass(this._container, idMatch ? idMatch[idMatch.length - 1] : elementId);
                if (!this.get_showActionBar())
                    $(this._container).addClass('ActionBarHidden');
                if (!this.get_showDescription())
                    $(this._container).addClass('DescriptionHidden');
            }
            if (this.get_filterSource() && this.get_filterSource() != 'Context') {
                var source = this.get_master();
                if (source) {
                    this._hasParent = true;
                    source.add_selected(this._filterSourceSelectedHandler);
                    //if (this.get_transaction() == 'Supported')
                    //    if (!isNullOrEmpty(source.get_transaction())) {
                    //        this.set_transaction(source.get_transaction() != 'Supported' ? source.get_transaction() : null);
                    //        this._forceVisible = this.get_transaction() && source.inserting();
                    //    }
                    if (source._pendingSelectedEvent && !_touch) {
                        this._source = source;
                        var self = this;
                        this._afterUpdateTimerId = setInterval(function () {
                            self._afterUpdate();
                        }, 250);
                    }
                    else if (!this._forceVisible)
                        this._hidden = true;
                }
                else {
                    source = $get(this.get_filterSource());
                    if (source) $addHandler(source, 'change', this._filterSourceSelectedHandler);
                    this.set_domFilterSource(source);
                }
                if (this._externalFilter.length == 0)
                    this._createExternalFilter();
                if (this._filter.length == 0)
                    if (!this._source) this.applyExternalFilter();
            }
            else {
                this._hidden = !this._evaluateVisibility();
                if (this._hidden)
                    this._updateLayoutContainerVisibility(false);
            }
            //if (this.get_transaction() == 'Supported')
            //    this.set_transaction(null);
            if (!_touch && this.get_modalAnchor() && !this.get_isModal())
                this._initializeModalPopup();
            if (source != null && this.get_autoHide() != Web.AutoHideMode.Nothing)
                this._updateLayoutContainerVisibility(false);
            //if (this.get_startCommandName() == 'UseTransaction') {
            //    this._usesTransaction = true;
            //    this.set_startCommandName(null);
            //}
            //if (this.get_startCommandName() == 'DetectTransaction')
            //    if (!this.get_transaction()) {
            //        if (source && _app.isInstanceOfType(source)) {
            //            source.remove_selected(this._filterSourceSelectedHandler);
            //            clearInterval(this._afterUpdateTimerId);
            //        }
            //        return;
            //    }
            //    else
            //        this.set_startCommandName(null);
            var commandLine = (_app.get_commandLine() || '').replace(/#.+?$/, ''),
                command = commandLine.match(/_commandName=(.+?)&_commandArgument=(.*?)(&|$)/);
            if (!command)
                command = commandLine.match(/_command=(.+?)&_argument=(.*?)(&|$)/);
            if (command && (isNullOrEmpty(this.get_startCommandName()) || __designer()) && !this.get_filterSource() && !this.get_isModal() && !this.get_lookupField()) {
                var tc = commandLine.match(/_controller=(\w+)/);
                var tv = commandLine.match(/_view=(\w+)/);
                if ((!tc || tc[1] == this.get_controller()) && (!tv || tv[1] == this.get_view())) {
                    this._trySecondCommand = !isNullOrEmpty(tc);
                    this.set_startCommandName(command[1]);
                    this.set_startCommandArgument(command[2]);
                    if (!isNullOrEmpty(this._viewId)) this._replaceTriggerViewId = this._viewId;
                    //this._skipRender = true;
                    this._skipTriggeredView = true;
                    var syncKey = commandLine.match(/\b_sync=(.+?)(&|$)/);
                    if (syncKey)
                        this._syncKey = syncKey[1].split(',');
                }
                else if (!this._filterSource) {
                    this._visibleWhen = 'false';
                    this._updateLayoutContainerVisibility(false);
                }
            }
            if (this.useCase('$app'))
                return;
            if (this.get_startCommandName()) {
                this.set_searchOnStart(false);
                this.set_lastCommandName(this.get_startCommandName());
                this.set_lastCommandArgument(this.get_startCommandArgument());
                if (this.get_startCommandName().match(/New|Edit|Select/))
                    this.set_viewId(this.get_startCommandArgument());
                this.set_startCommandName(null);
                this.set_startCommandArgument(null);
                this._rows = [];
                if (!_touch && !this._survey)
                    this._loadPage();

            }
            else
                this.loadPage();
            if (_touch)
                this.mobileUpdated();
        },
        _afterUpdate: function () {
            if (this._delayedLoading && this._source._pendingSelectedEvent || this._source._isBusy) return;
            clearInterval(this._afterUpdateTimerId);
            var source = this._source;
            this._source = null;
            this._filterSourceSelected(source, Sys.EventArgs.Empty, true);
        },
        dataAttr: function (element, name, value) {
            var $elem = $(element);
            var dataAttrName = 'data-' + name;
            if (!value) {
                value = $elem.attr(dataAttrName);
                if (!value)
                    value = $elem.attr('factory:' + name);
                return value;
            }
            else
                $elem.attr(dataAttrName, value);
        },
        _updateLayoutContainerVisibility: function (visible) {
            var that = this;
            if (_touch) {
                that._hidden = !visible;
                if (visible && that._hiddenEcho) {
                    that._hiddenEcho = false;
                    $('.app-echo[data-for="' + that._id + '"]').show();
                }
                return;
            }
            if (that._forceVisible) visible = true;
            var c = that._element.parentNode;
            if (isNullOrEmpty(c.getAttribute('data-visibility-controller')))
                c.setAttribute('data-visibility-controller', that.get_id());
            var activator = !isNullOrEmpty(that.dataAttr(c, 'activator')) ? that.dataAttr(c, 'activator').match(/^\s*(\w+)\s*\|\s*(.+?)\s*(\|\s*(.+)\s*)?$/) : null;
            if (that.get_autoHide() == Web.AutoHideMode.Self) {
                that._element.style.display = visible ? '' : 'none';
                if (!visible)
                    visible = $(that._element).siblings().is(':visible');
                var tabBar = c.parentNode.childNodes[0];
                while (tabBar.tagName != 'DIV')
                    tabBar = tabBar.nextSibling;
                if (activator && activator[1] == 'Tab' && Sys.UI.DomElement.containsCssClass(tabBar, 'TabBar')) {
                    if (c.getAttribute('data-visibility-controller') == that.get_id()) {
                        var tabIndex = -1;
                        for (var i = 0; i < c.parentNode.childNodes.length; i++) {
                            var n = c.parentNode.childNodes[i];
                            if (n.className && Sys.UI.DomElement.containsCssClass(n, 'TabBody')) tabIndex++;
                            if (c == n) break;

                        }
                        if (tabIndex != -1) {
                            var menuCells = tabBar.getElementsByTagName('td');
                            var menuCell = menuCells[tabIndex];
                            menuCell.style.display = visible ? '' : 'none';
                            if (!visible && Sys.UI.DomElement.containsCssClass(menuCell, 'Selected')) {
                                var links = tabBar.getElementsByTagName('a');
                                for (i = 0; i < menuCells.length; i++) {
                                    if ($common.getVisible(menuCells[i])) {
                                        $find(c.parentNode.id + '$ActivatorMenu').select(i, links[i]);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    that.dataAttr(c, 'hidden', !visible);
            }
            else {
                if (activator && activator[1] == 'SideBarTask')
                    that.dataAttr(c, 'hidden', true);
                while (c) {
                    if (!isNullOrEmpty(that.dataAttr(c, 'flow'))) {
                        c.style.display = visible ? '' : 'none';
                        break;
                    }
                    c = c.parentNode;
                }
            }
            var sideBar = $get('PageContentSideBar');
            if (sideBar && activator) {
                var tasks = sideBar.getElementsByTagName('a');
                for (i = 0; i < tasks.length; i++) {
                    var l = tasks[i];
                    if (Sys.UI.DomElement.containsCssClass(l.parentNode, 'Task') && l.innerHTML == activator[2]) {
                        l.parentNode.style.display = visible ? '' : 'none';
                        break;
                    }
                }
            }
            if (_app._loaded)
                _body_resize();
        },
        loadPage: function (showWait) {
            if (_touch) return;
            var displayed = this.get_isDisplayed();
            this._showWait(!displayed);
            if (this.get_mode() != Web.DataViewMode.View || (this.get_lookupField() || !(this._delayedLoading = !displayed))) {
                if (!this._source)
                    this._loadPage();
            }
            else if (!Array.contains(_app._delayedLoadingViews, this)) {
                Array.add(_app._delayedLoadingViews, this);
                _app._startDelayedLoading();
            }
        },
        get_isDisplayed: function () {
            if (this._hidden) return false;
            var elem = this.get_element(),
                node;
            if (!elem)
                return false;
            node = elem && elem.parentNode;
            while (node != null) {
                if (node.getAttribute && node.tagName != 'TABLE' && this.dataAttr(node, 'activator') && !node._activated)
                    return false;
                if (node.style && node.style.display == 'none')
                    return false;
                node = node.parentNode;
            }
            return true;
        },
        goToPage: function (pageIndex, absolute) {
            if (this._isBusy) return;
            if (absolute)
                this._pageOffset = 0;
            //if (this.get_isDataSheet())
            //    delete this._viewColumnSettings;
            this.set_pageIndex(pageIndex);
            this._loadPage();
        },
        sort: function (sortExpression) {
            if (this._isBusy) return;
            this._clearCache();
            if (this.get_sortExpression() == sortExpression) sortExpression = '';
            this.set_sortExpression(sortExpression);
            if (this.get_selectedKey().length > 0) {
                this._saveViewVitals()
                this._sync();
            }
            else {
                this.set_pageIndex(0);
                this._loadPage();
            }
        },
        groupExpression: function () {
            var groupExpression = this.get_groupExpression();
            if (groupExpression)
                return groupExpression.trim().split(_app._simpleListRegex);
            return null;
        },
        groupBy: function () {
            var that = this;
            $(that.groupExpression()).each(function () {
                var f = that.findField(this);
                if (f)
                    f.GroupBy = true;
            });
        },
        applyFilterByIndex: function (fieldIndex, valueIndex) {
            if (valueIndex == -1)
                this.removeFromFilterByIndex(fieldIndex);
            else {
                var filterField = this._allFields[fieldIndex];
                var field = this.findFieldUnderAlias(filterField);
                this.applyFilter(filterField, valueIndex >= 0 ? '=' : null, valueIndex >= 0 ? field._listOfValues[valueIndex] : null);
            }
        },
        findFieldUnderAlias: function (aliasField) {
            if (typeof aliasField == 'string')
                aliasField = this.findField(aliasField);
            if (aliasField.Hidden)
                //for (var i = 0; i < this._allFields.length; i++)
                //    if (this._allFields[i].AliasIndex == aliasField.Index) return this._allFields[i];
                return this._allFields[aliasField.OriginalIndex];
            return aliasField;
        },
        removeFromFilterByIndex: function (index) {
            if (index == -1)
                this.removeQuickFind();
            else {
                field = this._allFields[index];
                this.removeFromFilter(field);
            }
            this.sync();
        },
        removeQuickFind: function () {
            var that = this;
            $(that._filter).each(function (index) {
                if (this.match(/^_quickfind_\:\~/))
                    that._filter.splice(index, 1);
            });
        },
        removeFromFilter: function (field) {
            for (var i = 0; i < this._filter.length; i++) {
                if (this._filter[i].match(/^(\w+):/)[1] == field.Name) {
                    Array.removeAt(this._filter, i);
                    break;
                }
            }
        },
        clearFilter: function (force) {
            for (var i = 0; i < this._allFields.length; i++) {
                var f = this._allFields[i];
                var af = this.findFieldUnderAlias(f);
                if (this.filterOf(f) != null && !this.filterIsExternal(f.Name))
                    this.removeFromFilter(f);
            }
            if (force) {
                var qfe = this.get_quickFindElement();
                if (qfe != null) {
                    qfe.value = '';
                    this.quickFind();
                }
                else {
                    this.set_quickFindText(null);
                    this._executeQuickFind(null);
                }
            }
        },
        beginFilter: function () {
            this._filtering = true;
        },
        endFilter: function () {
            this._filtering = false;
            this.refreshData();
            //this.set_pageIndex(-2);
            //this._loadPage();
        },
        applyFilter: function (field, operator, value) {
            this._clearCache();
            this.removeFromFilter(field);
            if (operator == ':') {
                if (value) this._filter.push(field.Name + ':' + value);
            }
            else if (operator)
                this._filter.push((operator == '~' ? '_quickfind_' : field.Name) + ':' + operator + this.convertFieldValueToString(field, value));
            var keepFieldValues = (this._filter.length == 1 && this._filter[0].match(/(\w+):/)[1] == field.Name);
            field = this.findFieldUnderAlias(field);
            for (var i = 0; i < this._allFields.length; i++)
                if (!keepFieldValues || this._allFields[i].Name != field.Name)
                    this._allFields[i]._listOfValues = null;
            if (this._filtering != true)
                this._sync();
        },
        applyExternalFilter: function (preserveFilter) {
            if (!preserveFilter || !this._filter) this._filter = [];
            this._selectedRowIndex = null;
            for (var i = 0; i < this._externalFilter.length; i++) {
                var filterItem = this._externalFilter[i];
                if (preserveFilter) {
                    for (var j = 0; j < this._filter.length; j++) {
                        if (this._filter[j].startsWith(filterItem.Name + ':=')) {
                            Array.removeAt(this._filter, j);
                            break;
                        }
                    }
                }
                Array.add(this._filter, filterItem.Name + ':=' + filterItem.Value);
            }
        },
        applyFieldFilter: function (fieldIndex, func, values, defer) {
            if (fieldIndex == null)
                fieldIndex = this._filterFieldIndex;
            if (!func)
                func = this._filterFieldFunc;
            var field = this._allFields[fieldIndex];
            this.removeFromFilter(field);
            $(this._allFields).each(function () {
                this._listOfValues = null;
            });
            //var filter = field.Name + ':';
            var filter = String.format('{0}:', field.Name, field.Type);

            if (values && values[0] == resourcesHeaderFilter.EmptyValue)
                values[0] = String.jsNull;
            if (!values)
                filter += func + '$\0';
            else if (func == '$between')
                filter += '$between$' + this.convertFieldValueToString(field, values[0]) + '$and$' + this.convertFieldValueToString(field, values[1]) + '\0';
            else
                for (var i = 0; i < values.length; i++)
                    filter += func + (func.startsWith('$') ? '$' : '') + this.convertFieldValueToString(field, values[i]) + '\0';
            if (filter.indexOf('\0') > 0) Array.add(this._filter, filter);
            //        if (!defer) {
            //            this.set_pageIndex(-2);
            //            this._loadPage();
            //        }
            if (!defer)
                this.refreshData();
            this._forgetSelectedRow(true);
        },
        get_fieldFilter: function (field, extractFunction) {
            for (var i = 0; i < this._filter.length; i++) {
                var m = this._filter[i].match(/(\w+):(\*|\$\w+\$|=|~|>=?|<(=|>){0,1})([\s\S]*)/);
                if (m[1] == field.Name) {
                    if (extractFunction) {
                        var s = m[2];
                        return s.startsWith('$') ? s.substring(0, s.length - 1) : s;
                    }
                    else
                        return m[4];
                }
            }
            return null;
        },
        _createFieldInputExtender: function (type, field, input, index) {
            var c = null;
            if (field.Type.startsWith('DateTime')) {
                c = $create(AjaxControlToolkit.CalendarBehavior, { id: String.format('{0}_{1}Calendar{2}_{3}', this.get_id(), type, field.Index, index), button: input.nextSibling }, null, null, input);
                c.set_format(field.DataFormatString.match(/\{0:([\s\S]*?)\}/)[1]);
            }
            else if (field.AllowQBE && field.Type == 'String' && field.AllowAutoComplete != false) {
                c = $create(Web.AutoComplete, {
                    'completionInterval': 500,
                    'contextKey': String.format('{0}:{1},{2}', type, this.get_id(), field.Name),
                    'delimiterCharacters': ';',
                    'id': String.format('{0}_{1}AutoComplete{2}_{3}', this.get_id(), type, field.Index, index),
                    'minimumPrefixLength': field.AutoCompletePrefixLength == 0 ? 1 : field.AutoCompletePrefixLength,
                    'serviceMethod': 'GetListOfValues',
                    'servicePath': this.get_servicePath(),
                    'useContextKey': true,
                    'enableCaching': type != 'SearchBar',
                    'typeCssClass': type
                },
                    null, null, input);
                c._updateClearButton();
            }
            return c;
        },
        showFieldFilter: function (fieldIndex, func, text) {
            if (!this._filterExtenders) this._filterExtenders = [];
            var field = this._allFields[fieldIndex];
            var filter = this.get_fieldFilter(field);
            if (filter) {
                var vm = filter.match(/^([\s\S]*?)(\0|$)/);
                if (vm) filter = vm[1];
            }
            filter = filter && !String.isJavaScriptNull(filter) ? filter.split(_app._listRegex) : [''];
            this._filterFieldIndex = fieldIndex;
            this._filterFieldFunc = func;
            var filterElement = this._filterElement = document.createElement('div');
            filterElement.id = this.get_id() + '$FieldFilter';
            filterElement.className = 'FieldFilter';
            var sb = new Sys.StringBuilder();
            var button = field.Type.startsWith('DateTime') ? '<a class="Calendar" href="javascript:" onclick="return false">&nbsp;</a>' : '';
            sb.appendFormat('<div class="Field"><div class="Label"><span class="Name">{0}</span> <span class="Function">{1}</span></div><div class="Value"><input type="text" value="{2}"/>{3}</div></div>', field.HeaderText, text.toLowerCase(), _app.htmlAttributeEncode(field.format(this.convertStringToFieldValue(field, filter[0]))), button);
            if (func == '$between')
                sb.appendFormat('<div class="Field"><div class="Label"><span class="Function">{0}</span></div><div class="Value"><input type="text" value="{1}"/>{2}</div></div>', resourcesDataFiltersLabels.And, _app.htmlAttributeEncode(filter[1] ? field.format(this.convertStringToFieldValue(field, filter[1])) : ''), button);
            sb.appendFormat('<div class="Buttons"><button onclick="$find(\'{0}\').closeFieldFilter(true);return false">{1}</button><button onclick="$find(\'{0}\').closeFieldFilter(false);return false">{2}</button></div>', this.get_id(), resourcesModalPopup.OkButton, resourcesModalPopup.CancelButton);
            filterElement.innerHTML = sb.toString();
            //document.body.appendChild(this._filterElement);
            this._appendModalPanel(filterElement);
            this._filterPopup = $create(AjaxControlToolkit.ModalPopupBehavior, { 'id': this.get_id() + '$FilterPopup', PopupControlID: filterElement.id, DropShadow: true, BackgroundCssClass: 'ModalBackground' }, null, null, this._container.getElementsByTagName('a')[0]);
            var inputList = filterElement.getElementsByTagName('input');
            for (var i = 0; i < inputList.length; i++) {
                var input = inputList[i];
                var c = this._createFieldInputExtender('Filter', field, input, i);
                if (c) Array.add(this._filterExtenders, c);
            }
            this._saveTabIndexes();
            this._filterPopup.show();
            //inputList[0].focus();
            Sys.UI.DomElement.setFocus(inputList[0]);
            $(filterElement).find('input:text').keydown(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    $(filterElement).find('button:first').click();
                }
            });
        },
        closeFieldFilter: function (apply) {
            var inputList = this._filterElement.getElementsByTagName('input');
            var values = [];
            if (apply) {
                var field = this._allFields[this._filterFieldIndex];
                for (var i = 0; i < inputList.length; i++) {
                    var input = inputList[i];
                    if (isBlank(input.value)) {
                        alert(resourcesValidator.RequiredField);
                        Sys.UI.DomElement.setFocus(input);
                        //input.focus();
                        return;
                    }
                    else {
                        this._formatSearchField(input, this._filterFieldIndex);
                        var v = { NewValue: input.value.trim() };
                        var error = this._validateFieldValueFormat(field, v);
                        if (error) {
                            alert(error);
                            Sys.UI.DomElement.setFocus(input);
                            //input.focus();
                            //input.select();
                            return;
                        }
                        else
                            Array.add(values, field.Type.startsWith('DateTime') ? input.value.trim() : v.NewValue);
                    }
                }
            }
            this._disposeFieldFilter();
            this._restoreTabIndexes();
            if (apply)
                this.applyFieldFilter(null, null, values);
        },
        _disposeSearchBarExtenders: function () {
            if (this._searchBarExtenders) {
                for (var i = 0; i < this._searchBarExtenders.length; i++)
                    this._searchBarExtenders[i].dispose();
                Array.clear(this._searchBarExtenders);
            }
        },
        _disposeFieldFilter: function () {
            if (this._filterExtenders) {
                for (var i = 0; i < this._filterExtenders.length; i++)
                    this._filterExtenders[i].dispose();
                Array.clear(this._filterExtenders);
            }
            if (this._filterElement) {
                this._filterPopup.hide();
                this._filterPopup.dispose();
                this._filterPopup = null;
                this._filterElement.parentNode.removeChild(this._filterElement);
                delete this._filterElement;
            }
        },
        _showSearchBarFilter: function (fieldIndex, visibleIndex) {
            this._searchBarVisibleIndex = visibleIndex;
            if (fieldIndex == -1) {
                var elem = this._get('$SearchBarValue$', visibleIndex);
                elem.value = '';
                this._searchBarFuncChanged(visibleIndex);
                this._searchBarVisibleIndex = null;
            }
            else
                this.showCustomFilter(fieldIndex);
        },
        _appendModalPanel: function (elem) {
            var element = this.get_element();
            if (element)
                element.appendChild(elem);
        },
        showCustomFilter: function (fieldIndex) {
            var field = this._allFields[fieldIndex];
            var panel = this._customFilterPanel = document.createElement('div');
            //this.get_element().appendChild(panel);
            this._appendModalPanel(panel);
            panel.className = 'CustomFilterDialog';
            panel.id = this.get_id() + '_CustomFilterPanel';
            var sb = new Sys.StringBuilder();
            sb.appendFormat('<div><span class="Highlight">{0}</span> {1}:</div>', _app.htmlEncode(field.Label), resourcesDataFiltersLabels.Includes);
            sb.append('<table cellpadding="0" cellspacing="0">');
            sb.appendFormat('<tr><td colspan="2"><div id="{0}$CustomFilterItemList${1}" class="CustomFilterItems">{2}</div></td></tr>', this.get_id(), fieldIndex, resources.Common.WaitHtml);
            sb.appendFormat('<tr><td></td><td align="right"><button id="{0}Ok">{1}</button> <button id="{0}Cancel">{2}</button></td></tr>', this.get_id(), resourcesModalPopup.OkButton, resourcesModalPopup.CancelButton);
            sb.append('</table>');
            panel.innerHTML = sb.toString();
            sb.clear();
            this._customFilterField = field;
            this._customFilterModalPopupBehavior = $create(AjaxControlToolkit.ModalPopupBehavior, {
                OkControlID: this.get_id() + 'Ok', CancelControlID: this.get_id() + 'Cancel', OnOkScript: String.format('$find("{0}").applyCustomFilter()', this.get_id()), OnCancelScript: String.format('$find("{0}").closeCustomFilter()', this.get_id()),
                PopupControlID: panel.id, DropShadow: true, BackgroundCssClass: 'ModalBackground'
            }, null, null, this._container.getElementsByTagName('a')[0]);
            this._customFilterModalPopupBehavior.show();
            $addHandler(document.body, 'keydown', this._bodyKeydownHandler);
            var originalField = this.findFieldUnderAlias(field);
            //if (originalField._listOfValues && this._searchBarVisibleIndex == null)
            //    this._onGetFilterListOfValuesComplete(originalField._listOfValues, { 'fieldName': field.Name });
            //else
            //    this._loadFilterListOfValues(field.Name);
            this._loadFilterListOfValues(field.Name);
        },
        _loadFilterListOfValues: function (fieldName) {
            this._busy(true);
            var lc = this.get_lookupContext();
            this._invoke('GetListOfValues', this._createArgsForListOfValues(fieldName), Function.createDelegate(this, this._onGetFilterListOfValuesComplete), { 'fieldName': fieldName });
        },
        _renderFilterOption: function (sb, field, v, index, selected) {
            var item = this._findItemByValue(field, v);
            if (item)
                v = item[1];
            sb.appendFormat('<tr><td class="Input"><input type="checkbox" id="{0}$CustomFilterItem{1}${2}"{4}/></td><td class="Label"><label for="{0}$CustomFilterItem{1}${2}">{3}</label></td></tr>', this.get_id(), field.Index, index, v, selected ? ' checked="checked"' : '');
        },
        _onGetFilterListOfValuesComplete: function (result, context) {
            this._busy(false);
            var field = this.findField(context.fieldName);
            if (result[result.length - 1] == null) {
                Array.insert(result, 0, result[result.length - 1]);
                Array.removeAt(result, result.length - 1);
            }
            var itemsElem = this._get('$CustomFilterItemList$', field.Index);
            if (!itemsElem) return;

            var currentFilter = this._searchBarVisibleIndex != null ? this._createSearchBarFilter(true) : this._filter,
                customFilter = null;
            this.findFieldUnderAlias(field)._listOfValues = result;
            for (var i = 0; i < currentFilter.length; i++) {
                var m = currentFilter[i].match(_app._fieldFilterRegex);
                if (m[1] == field.Name) {
                    customFilter = m[2];
                    break;
                }
            }
            var listOfValues = null,
                func = '$in$';
            if (customFilter) {
                m = customFilter.match(_app._filterRegex);
                if (m) {
                    if (m[1].match(/\$(in|notin|between)\$/)) {
                        func = m[1];
                        listOfValues = m[3].split(_app._listRegex);
                    }
                    else
                        listOfValues = [m[3]];
                    for (i = 0; i < listOfValues.length; i++) {
                        var v = listOfValues[i];
                        if (String.isJavaScriptNull(v))
                            listOfValues[i] = resourcesHeaderFilter.EmptyValue;
                        else {
                            v = this.convertStringToFieldValue(field, v);
                            listOfValues[i] = field.format(v);
                        }
                    }
                }
            }

            var sb = new Sys.StringBuilder();

            sb.appendFormat('<table class="FilterItems" cellpadding="0" cellspacing="0"><tr><td><input type="CheckBox" onclick="$find(\'{0}\')._selectAllFilterItems({1})" id="{0}$CustomFilterItemList$SelectAll"/></td><td><label for="{0}$CustomFilterItemList$SelectAll">{2}</label></td></tr>', this.get_id(), field.Index, resourcesDataFiltersLabels.SelectAll);

            /*for (i = 0; i < result.length; i++) {
            v = result[i];
            if (v == null)
            v = Web.DataViewresourcesHeaderFilter.EmptyValue;
            else {
            v = _app.htmlEncode(field.format(v));
            if (v == '') v = Web.DataViewresourcesHeaderFilter.BlankValue;
            }
            var selected = listOfValues && ((func == '$in$' && Array.contains(listOfValues, v)) || (func == '$notin$' && !Array.contains(listOfValues, v)));
            sb.appendFormat('<tr><td class="Input"><input type="checkbox" id="{0}$CustomFilterItem{1}${2}"{4}/></td><td class="Label"><label for="{0}$CustomFilterItem{1}${2}">{3}</label></td></tr>', this.get_id(), field.Index, i, v, selected ? ' checked="checked"' : '');
            }
            */

            var selectedValues = [],
                selectedStrings = [],
                otherValues = [],
                otherStrings = [],
                originalField = this.findFieldUnderAlias(field)
            for (i = 0; i < result.length; i++) {
                v = result[i];
                var v2 = v;
                if (v == null)
                    v = resourcesHeaderFilter.EmptyValue;
                else {
                    v = _app.htmlEncode(field.format(v));
                    if (v == '') v = resourcesHeaderFilter.BlankValue;
                }
                var selected = listOfValues && ((func == '$in$' && Array.contains(listOfValues, v)) || (func == '$notin$' && !Array.contains(listOfValues, v)));
                if (selected) {
                    Array.add(selectedStrings, v);
                    Array.add(selectedValues, v2);
                }
                else {
                    Array.add(otherStrings, v);
                    Array.add(otherValues, v2);
                }
            }
            for (i = 0; i < selectedStrings.length; i++)
                this._renderFilterOption(sb, field, selectedStrings[i], i, true);
            for (i = 0; i < otherStrings.length; i++)
                this._renderFilterOption(sb, field, otherStrings[i], i + selectedStrings.length, false);

            sb.append('</table>');
            itemsElem.innerHTML = sb.toString();
            //itemsElem.getElementsByTagName('input')[0].focus();
            Sys.UI.DomElement.setFocus(itemsElem.getElementsByTagName('input')[0]);
            if (selectedValues.length > 0) {
                originalField._listOfValues = [];
                Array.addRange(originalField._listOfValues, selectedValues);
                Array.addRange(originalField._listOfValues, otherValues);
            }
        },
        _selectAllFilterItems: function (fieldIndex) {
            var itemsElem = this._get('$CustomFilterItemList$', fieldIndex);
            var list = itemsElem.getElementsByTagName('input');
            for (var i = 1; i < list.length; i++)
                list[i].checked = list[0].checked;
        },
        applyCustomFilter: function () {
            var field = this._customFilterField;
            var searchBarValue = this._searchBarVisibleIndex != null ? this._get('$SearchBarValue$', this._searchBarVisibleIndex) : null;
            var searchBarFunc = searchBarValue ? this._get('$SearchBarFunction$', this._searchBarVisibleIndex) : null;
            this.removeFromFilter(field);
            var filter = null
            var itemsElem = this._get('$CustomFilterItemList$', field.Index);
            var list = itemsElem.getElementsByTagName('input');
            var originalField = this.findFieldUnderAlias(field);
            var numberOfOptions = 0;
            for (var i = 1; i < list.length; i++)
                if (list[i].checked) {
                    if (!filter)
                        filter = '';
                    else
                        filter += '$or$';
                    filter += this.convertFieldValueToString(field, originalField._listOfValues[i - 1]);
                    numberOfOptions++;

                }
            if (filter && (numberOfOptions <= 10 || numberOfOptions != list.length - 1)) {
                if (numberOfOptions <= 10 || numberOfOptions <= (list.length - 1) / 2) {
                    if (searchBarValue)
                        searchBarFunc.value = '$in,true';
                    else
                        Array.add(this._filter, String.format('{0}:$in${1}\0', this._customFilterField.Name, filter));
                }
                else {
                    filter = null;
                    for (i = 1; i < list.length; i++)
                        if (!list[i].checked) {
                            if (!filter)
                                filter = '';
                            else
                                filter += '$or$';
                            filter += this.convertFieldValueToString(field, originalField._listOfValues[i - 1]);
                        }
                    if (searchBarValue)
                        searchBarFunc.value = '$notin,true';
                    else
                        Array.add(this._filter, String.format('{0}:$notin${1}\0', this._customFilterField.Name, filter));
                }
            }
            else
                filter = null;

            for (i = 0; i < this._allFields.length; i++) {
                var f = this._allFields[i];
                if (f != originalField)
                    f._listOfValues = null;
            }
            if (searchBarValue) {
                searchBarValue.value = filter ? filter : '';
                this._searchBarFuncChanged(this._searchBarVisibleIndex);
            }
            //        else {
            //            this.set_pageIndex(-2);
            //            this._loadPage();
            //        }
            else {
                this._forceSync();
                this.refreshData();
            }
            this.closeCustomFilter();
            //this._forgetSelectedRow(true);
        },
        //    applyCustomFilter2: function () {
        //        this.removeFromFilter(this._customFilterField);
        //        var iterator = /\s*(=|<={0,1}|>={0,1}|)\s*([\S\s]+?)\s*(,|;|$)/g;
        //        var filter = this._customFilterField.Name + ':';
        //        var s = $get(this.get_id() + '_Query').value;
        //        var match = iterator.exec(s);
        //        while (match) {
        //            if (!isBlank(match[2]))
        //                filter += (match[1] ? match[1] : (this._customFilterField.Type == 'String' ? '*' : '=')) + match[2] + '\0';
        //            match = iterator.exec(s);
        //        }
        //        if (filter.indexOf('\0') > 0) Array.add(this._filter, filter);
        //        this.set_pageIndex(-2);
        //        this._loadPage();
        //        this.closeCustomFilter();
        //    },
        closeCustomFilter: function () {
            if (this._customFilterModalPopupBehavior) {
                this._customFilterModalPopupBehavior.dispose();
                this._customFilterModalPopupBehavior = null;
                this._customFilterField = null;
            }
            if (this._customFilterPanel) {
                this._customFilterPanel.parentNode.removeChild(this._customFilterPanel);
                delete this._customFilterPanel;
            }
            $removeHandler(document.body, 'keydown', this._bodyKeydownHandler);
            this._customFilterField = null;
            this._searchBarVisibleIndex = null;
        },
        convertFieldValueToString: function (field, value) {
            //        if (field.Type == 'String')
            //            return value != null && typeof (value) != String ? value.toString() : value;
            //        else {
            //            if (value != null && typeof (value) == 'string')
            //                value = this.convertStringToFieldValue2(field, value);
            //            return String.format('%js%{0}', Sys.Serialization.JavaScriptSerializer.serialize(value));
            //        }
            if (typeof (value) == 'string' && (value.match(_app._listRegex) || value.startsWith('%js%')))
                return value;
            if (field.Type != 'String' && value != null && typeof (value) == 'string')
                value = this.convertStringToFieldValue2(field, value);
            if (Date.isInstanceOfType(value))
                value = _app.stringifyDate(value/*, field*/);
            //value = new Date(value - value.getTimezoneOffset() * 60 * 1000);
            return String.format('%js%{0}', _serializer.serialize(value));
        },
        convertFieldValueToString2: function (field, value) {
            if (field.Type.startsWith('DateTime') && !isNullOrEmpty(field.DataFormatString)) {
                if (value == null)
                    return null;
                else
                    return String.localeFormat(field.DataFormatString, value);
            }
            else {
                if (field.Type == 'Boolean') {
                    if (value == null)
                        return null;
                    else
                        return value.toString();
                }
                else {
                    //if (field.Type == 'Decimal' || field.Type == 'Single')
                    //    return String.localeFormat('{0:N6}', value);
                    //else
                    return value.toString();
                }
            }
        },
        convertStringToFieldValue: function (field, value) {
            if (typeof value !== 'string') return value;
            if (value != null && value.startsWith('%js%')) {
                value = _serializer.deserialize(value.substring(4));
                if (_app.isISO8601DateString(value))
                    value = new Date(value);
                // if (Date.isInstanceOfType(value))
                //     value = new Date(value.getTime() + value.getTimezoneOffset() * 60 * 1000);
                return value;
            }
            else
                return this.convertStringToFieldValue2(field, value);
        },
        convertStringToFieldValue2: function (field, value) {
            if (value == null) return value;
            switch (field.Type) {
                case 'DateTime':
                    var d = field.DataFormatString && field.DataFormatString.length ? Date.parseLocale(value, field.DataFormatString.match(/\{0:([\s\S]*?)\}/)[1]) : Date.parse(value)
                    if (!isNaN(d) && d != null)
                        return d;
                    break;
                case 'SByte':
                case 'Byte':
                case 'Int16':
                case 'Int32':
                case 'UInt32':
                case 'Int64':
                case 'Single':
                case 'Double':
                case 'Decimal':
                case 'Currency':
                    var n = Number.parseLocale(value);
                    if (!isNaN(n))
                        return n;
                    break;
            }
            return value;
        },
        goToView: function (viewId) {
            if (!viewId && this._doneCallback) {
                this._doneCallback(this);
                return;
            }
            if (!isNullOrEmpty(this._replaceTriggerViewId) && this._replaceTriggerViewId == viewId) {
                if (this._skipTriggeredView) this._skipTriggeredView = false;
                location.replace(location.href)
                return;
            }
            var ditto = this._ditto && this._ditto.slice(0);
            this._clearCache(true);
            this._ditto = ditto;
            var lastFilter = this.get_filter();
            var lastGroup = this.get_view().Group;
            if (viewId == 'form')
                for (var i = 0; i < this.get_views().length; i++)
                    if (this.get_views()[i].Type == 'Form') {
                        viewId = this.get_views()[i].Id;
                        break;
                    }
            this._disposePendingUploads();
            this._detachBehaviors();
            if (!this.get_isForm() /*this.get_view().Type != 'Form'*/) {
                this._lastViewId = this.get_viewId();
                this._selectedRowIndex = 0;
            }
            var oldViewId = this.get_view().Id,
                viewChanged = oldViewId != viewId,
                wasForm,
                originalExtension = this.extension();
            if (viewChanged) {
                this._focusedFieldName = null;
                wasForm = this.get_isForm();
                if (!this.get_isGrid())
                    this.writeContext('vitals', null);
            }
            this.set_viewId(viewId);
            var v = this.get_view();
            if (v.Type != 'Form') {
                this._lastViewId = viewId;
                this._restorePosition();
                if (viewChanged && wasForm && _touch)
                    originalExtension._dispose(true);
            }
            this.set_pageIndex(-1);
            var viewFilter = v.Type == 'Form' ? this.get_selectedKeyFilter() : (!isNullOrEmpty(lastGroup) && this.get_view().Group == lastGroup || !viewChanged ? lastFilter : []);
            if (this._requestedFilter) {
                viewFilter = this._requestedFilter;
                this._requestedFilter = null;
            }
            var viewSortExpression = viewChanged ? null : this.get_sortExpression();
            if (this._requestedSortExpression) {
                viewSortExpression = this._requestedSortExpression;
                this._requestedSortExpression = null;
            }
            this.set_filter(viewFilter);
            this.set_sortExpression(viewSortExpression);
            this._loadPage();
            this._raiseSelectedDelayed = true;
            if (viewChanged) {
                this._scrollIntoView = true;
                this._focusedCell = null;
            }
        },
        filterOf: function (field) {
            var header = (!isNullOrEmpty(field.AliasName) ? field.AliasName : field.Name) + ':';
            for (var i = 0; i < this._filter.length; i++) {
                var s = this._filter[i];
                if (s.startsWith(header) && !s.match(':~'))
                    return this._filter[i].substr(header.length);
            }
            return null;
        },
        findField: function (query) {
            return this._mapOfAllFields[query];
            //for (var i = 0; i < this._allFields.length; i++) {
            //    var field = this._allFields[i];
            //    if (field.Name == query) return field;
            //}
            //return null;
        },
        findCategory: function (query) {
            for (var i = 0; i < this._categories.length; i++) {
                var c = this._categories[i];
                if (c.Id == query) return c;
            }
            return null;
        },

        _isInInstantDetailsMode: function () {
            return location.href.match(_app.DetailsRegex);
        },
        _closeInstantDetails: function () {
            if (this._isInInstantDetailsMode()) {
                if (resources.Lookup.ShowDetailsInPopup) {
                    close();
                    return true;
                }
            }
            return false;
        },
        _confirm: function (context, confirmCallback) {
            var iterator = /(_(\w+))\s*=\s*(.+?)(\s*\r|;|\n|$)/gi;
            var confirmation = context.action.Confirmation;
            var result = true;
            var m = iterator.exec(confirmation);
            if (m && !(__tf != 4)) {
                var survey = null;
                var controller = null;
                var view = '';
                var commandName = 'New';
                var commandArgument = '';
                while (m) {
                    var name = m[2];
                    var value = m[3];
                    if (name == 'survey')
                        survey = value;
                    if (name == 'controller')
                        controller = value;
                    if (name == 'view')
                        view = value;
                    if (name == 'commandName')
                        commandName = commandName;
                    if (name == 'commandArgument')
                        commandArgument = value;
                    if (name == 'width')
                        context.MaxWidth = value;
                    if (name == 'title')
                        context.WindowTitle = value;
                    m = iterator.exec(confirmation);
                }
                var dataView = this.get_isModal() ? null : findDataView(controller, 'Controller');
                if (dataView && !dataView.get_isModal()) {
                    var values = dataView._collectFieldValues();
                    result = dataView._validateFieldValues(values, true);
                    if (result) {
                        this._convertValuesToParameters(values);
                        this._paramValues = values;
                    }
                }
                else {
                    values = this._collectFieldValues();
                    var contextValues = context.Values = [],
                        servicePath = this.get_servicePath(),
                        baseUrl = this.get_baseUrl();
                    for (var i = 0; i < values.length; i++) {
                        var fv = values[i];
                        Array.add(contextValues, { Name: 'Context_' + fv.Name, Value: fv.Modified ? fv.NewValue : fv.OldValue });
                    }
                    if (survey)
                        _app.survey({ controller: survey, parent: this._id, confirmContext: context });
                    else
                        dataView = _app.showModal(this, controller, view, commandName, commandArgument, baseUrl, servicePath, [], { confirmContext: context, showSearchBar: this.get_showSearchBar(), tag: 'discard-changes-prompt-none' });
                    result = false;
                }
            }
            else {
                confirmation = this._parseText(confirmation, this.get_currentRow());
                if (confirmCallback)
                    confirmCallback(confirmation);
                else if (!confirm(confirmation))
                    result = false;
            }
            return result;
        },
        executeAction: function (scope, actionIndex, rowIndex, groupIndex, confirmed, action) {
            if (this._isBusy) return;
            _app.hideMessage();
            if (!action) {
                var isLookup = this.get_lookupField() != null;
                var actions = isLookup && this._lookupActionProcessing() ? null : (scope == 'ActionBar' ? this.get_actionGroups(scope)[groupIndex].Actions : this.get_actions(scope));
                if (actionIndex < 0 && actions) {
                    for (var i = 0; i < actions.length; i++)
                        if (this._isActionAvailable(actions[i], rowIndex)) {
                            actionIndex = i;
                            break;
                        }
                    if (actionIndex < 0) return;
                }
                action = actions ? actions[actionIndex] : null;
            }
            if (action && !isNullOrEmpty(action.Confirmation)) {
                if (this.get_isGrid() && !this.get_isDataSheet() && this.get_lastCommandName() != 'BatchEdit' && !isLookup)
                    this.executeRowCommand(rowIndex, 'Select');
                if (!confirmed && !this._confirm({ action: action, scope: scope, actionIndex: actionIndex, rowIndex: rowIndex, groupIndex: groupIndex }))
                    return;
            }
            var command = action ? action.CommandName : 'Select';
            var argument = action ? action.CommandArgument : null;
            var causesValidation = action ? action.CausesValidation : true;
            var path = action ? action.Path : null;
            this.executeRowCommand(rowIndex, command, argument, causesValidation, path);
        },
        delegateCommand: function (command, argument) {
            var that = this,
                isForm = that.get_isForm(),
                parent, parentDataViewId;
            if (_touch && (command.match(/New/) || command.match(/Select|Edit/) && argument && that._viewId != argument || command == 'Select' && argument && that.editing()) && isForm) {
                parentDataViewId = that._parentDataViewId;
                if (parentDataViewId) {
                    parent = findDataView(parentDataViewId);
                    if (parent.get_isForm())
                        parent = that;
                }
                else
                    parent = that;
                if (parent && that._controller == parent._controller)
                    _touch.pageInfo(that).deleted = true;
                else
                    parent = null;
            }
            return parent;
        },
        executeRowCommand: function (rowIndex, command, argument, causesValidation, path) {
            var that = this,
                primaryDataView,
                extension,
                args;
            //if (_app.touch && that.get_isForm()) {
            //    primaryDataView = _app.touch.toPrimaryDataView(that);
            //    if (that._id != primaryDataView._id && (command.match(/Select|New/) || (command == 'Edit' && argument && that._viewId != argument))) {
            //        _app.touch.pageInfo(that._id).deleted = true;
            //        primaryDataView.executeRowCommand(rowIndex, command, argument, causesValidation, path);
            //        return;
            //    }
            //};
            primaryDataView = that.delegateCommand(command, argument);
            if (primaryDataView && primaryDataView != that)
                primaryDataView.executeRowCommand(rowIndex, command, argument, causesValidation, path);
            else {
                extension = that.extension();
                if (rowIndex != null && rowIndex >= 0) {
                    that._selectedRowIndex = rowIndex;
                    that._raiseSelectedDelayed = !(command == 'Select' && isNullOrEmpty(argument));
                    if (extension)
                        that._selectedRow = that._rows[rowIndex];
                    that._selectKeyByRowIndex(rowIndex);
                }
                args = { commandName: command, commandArgument: argument ? argument : '', path: path, causesValidation: causesValidation ? true : false };
                if (!_touch && that.get_isDataSheet() && command.match(/Select|New|Edit|Duplicate/) && that._rows.length > 0) {
                    if (extension != null && !_app.Extensions.active(extension))
                        _app.Extensions.active(extension, true);
                    else if (!that._get_focusedCell())
                        that._startInputListenerOnCell(0, 0);
                }
                if (!that.executeCommand(args))
                    return;
                if (command == 'ClientScript') {
                    if (!that._raiseSelectedDelayed) {
                        var self = that;
                        setTimeout(function () {
                            self.refresh(true);
                        }, 10);
                    }
                }
                else if (command == 'Select' && argument == null && !that.get_isGrid())
                    that._render();
            }
        },
        rowCommand: function (row, command, argument, causesValidation, path) {
            //if (rowIndex != null && rowIndex >= 0) {
            //    this._selectedRowIndex = rowIndex;
            //    this._raiseSelectedDelayed = !(command == 'Select' && isNullOrEmpty(argument));
            //    this._selectKeyByRowIndex(rowIndex);
            //}
            this._raiseSelectedDelayed = !(command == 'Select' && isNullOrEmpty(argument));
            this._selectKeyByRow(row);
            var args = { commandName: command, commandArgument: argument ? argument : '', path: path, causesValidation: causesValidation ? true : false };
            if (this.get_isDataSheet() && command.match(/Select|New|Edit|Duplicate/)) {
                var extension = this.extension();
                if (extension != null && !_app.Extensions.active(extension))
                    _app.Extensions.active(extension, true);
            }
            if (!this.executeCommand(args))
                return;
            if (command == 'ClientScript') {
                if (!this._raiseSelectedDelayed) {
                    var self = this;
                    setTimeout(function () {
                        self.refresh(true);
                    }, 10);
                }
            }
            else if (command == 'Select' && argument == null && !this.get_isGrid())
                this._render();
        },
        _applySelectionFilter: function (q) {
            if (this.multiSelect() && this._selectedKeyList.length > 0 && this._keyFields.length == 1) {
                q.Filter = Array.clone(q.Filter);
                Array.add(q.Filter, String.format('{0}:$in${1}', this._keyFields[0].Name, this._selectedKeyList.join('$or$')));
            }
        },
        get_appRootPath: function () {
            var servicePath = this.get_servicePath();
            if (typeof (__cothost) != 'undefined') {
                if (__cothost == 'DotNetNuke' && servicePath.match(/DesktopModules\//i))
                    return servicePath.replace(/Service\.asmx$/i, '');
                if (__cothost == 'SharePoint' && servicePath.match(/_layouts\//i))
                    return servicePath.replace(/Service\.asmx$/i, '');
            }
            return '~/';
        },
        executeReport: function (args) {
            this._cancelConfirmation();
            var downloadArgs = {
                target: '',
                action: this.resolveClientUrl(this.get_appRootPath() + 'Report.ashx'),
                request: this._createParams(true),
                command: args.commandName,
                argument: args.commandArgument
            },
                request = downloadArgs.request;
            request.Controller = this.get_controller();
            request.View = this.get_viewId();
            var commandArgument = args.commandArgument;
            if (!isNullOrEmpty(commandArgument)) {
                if (commandArgument.startsWith('_'))
                    downloadArgs.f.target = commandArgument;
                var a = commandArgument.split(_app._simpleListRegex);
                commandArgument = a[0];
                if (a.length == 3) {
                    request.Controller = a[1];
                    request.View = a[2];
                }
            }
            downloadArgs.argument = commandArgument;
            if (request.Filter.length > 0 && this.get_viewType() != "Form") {
                var sb = new Sys.StringBuilder();
                if (this.useCase('$app'))
                    sb.append(this._filterDetailsText);
                else {
                    this._renderFilterDetails(sb, request.Filter);
                    var master = this.get_master();
                    if (master) {
                        var r = master.get_selectedRow();
                        for (var i = 0; i < master._allFields.length; i++) {
                            var field = master._allFields[i];
                            if (field.ShowInSummary && !field.OnDemand) {
                                field = master._allFields[field.AliasIndex];
                                if (!sb.isEmpty())
                                    sb.append(' ');
                                sb.appendFormat('{0} {1} {2}.', field.HeaderText, resourcesDataFiltersLabels.Equals, field.format(r[field.Index]));
                            }
                        }
                    }
                }
                request.FilterDetails = sb.toString();
            }
            if (this._viewMessages)
                request.FilterDetails = String.format('{0} {1}', this._viewMessages[this.get_viewId()], request.FilterDetails);
            if (request.FilterDetails)
                request.FilterDetails = request.FilterDetails.replace(/(<b class=\"String\">([\s\S]*?)<\/b>)/g, '"$2"').replace(/(&amp;)/g, '&').replace(/(<.+?>)|&nbsp;/g, '');
            if (request.FilterDetails && _touch)
                request.filterDetails = this.extension().filterStatus();
            this._applySelectionFilter(request);
            downloadArgs.actionArgs = this._createArguments(args);
            this._validateFieldValues(downloadArgs.actionArgs.Values, true);
            downloadArgs.actionArgs.ExternalFilter = request.ExternalFilter;
            this._showDownloadProgress();
            this._downloadFile(downloadArgs);
        },
        _downloadFile: function (args) {
            var form = this._get_dataRequestForm();
            form.target = args.target;
            form.action = args.action;
            $get('c', form).value = args.command;
            $get('a', form).value = args.argument;
            $get('q', form).value = _serializer.serialize(args.request);
            if (args.actionArgs)
                $get('aa', form).value = _app.htmlEncode(_serializer.serialize(args.actionArgs));
            form.submit();
        },
        _get_dataRequestForm: function () {
            var f = $get('_dataRequest_form');
            if (!f) {
                f = document.createElement('form');
                f.id = '_dataRequest_form';
                f.method = 'post';
                f.innerHTML = '<input type="hidden" name="q" id="q"/><input type="hidden" name="aa" id="aa"/><input type="hidden" name="c" id="c"/><input type="hidden" name="a" id="a"/>';
                document.body.appendChild(f);
            }
            return f;
        },
        _showDownloadProgress: function () {
            //if (_app.isSaaS()) return;
            this._busy(true);
            var that = this;
            return that._onLoadComplete(that.get_id()).done(function () {
                that._stopWaiting = false;
                that._busy(false);
                var execute = that._pendingExecuteComplete;
                if (execute) {
                    that._pendingExecuteComplete = null;
                    return that._onExecuteComplete(execute.result, execute.context);
                }
            });
        },
        _onLoadComplete: function (id) {
            var d = $.Deferred(),
                that = this,
                downloadToken = 'APPFACTORYDOWNLOADTOKEN';
            document.cookie = String.format('{0}={1}; path=/', downloadToken, encodeURIComponent(id));
            var downloadInterval = setInterval(function () {
                if (!isNullOrEmpty(document.cookie)) {
                    var cookies = document.cookie.split(';');
                    for (var i = 0; i < cookies.length; i++) {
                        var cookie = cookies[i].trim();
                        if (cookie.startsWith(downloadToken)) {
                            var cookieValue = cookie.substring(downloadToken.length + 1).split(',');
                            if (cookieValue.length == 2 && cookieValue[0] == that.get_id() || that._stopWaiting) {
                                document.cookie = String.format('{0}=; expires={1}; path=/', downloadToken, new Date(0).toUTCString());
                                clearInterval(downloadInterval);
                                d.resolve();
                            }
                        }
                    }
                }
            }, 500);
            return d.promise();
        },
        executeExport: function (args) {
            var downloadArgs = {
                target: args.commandName == 'ExportRss' ? '_blank' : '',
                action: this.resolveClientUrl(this.get_appRootPath() + 'Export.ashx'),
                command: args.commandName,
                argument: args.commandArgument,
                request: args
            }, params = this._createParams(true);
            this._applySelectionFilter(params);
            downloadArgs.request.Controller = this.get_controller();
            downloadArgs.request.View = this.get_viewId();
            downloadArgs.request.Filter = params.Filter;
            downloadArgs.request.SortExpression = params.SortExpression;
            this._downloadFile(downloadArgs);
        },
        _clearDynamicItems: function () {
            for (var i = 0; i < this._allFields.length; i++) {
                var f = this._allFields[i];
                if (f.DynamicItems) {
                    f.DynamicItems = null;
                    f.ItemCache = null;
                }
            }
        },
        _copyLookupValues: function (r, lf, nv, outputValues) {
            var values = outputValues || [], m;
            if (lf.Copy)
                while (m = _app._fieldMapRegex.exec(lf.Copy))
                    if (lf._dataView.findField(m[1])) {
                        if (m[2] == 'null')
                            Array.add(values, { 'name': m[1], 'value': null });
                        else
                            if (r) {
                                var f = this.findField(m[2]);
                                if (f) Array.add(values, { 'name': m[1], 'value': r[f.Index] });
                            }
                            else if (nv) {
                                for (var i = 0; i < nv.length; i++) {
                                    if (nv[i].Name == m[2]) {
                                        Array.add(values, { 'name': m[1], 'value': nv[i].NewValue });
                                        break;
                                    }
                                }
                            }
                    }
            if (outputValues) return;
            lf._dataView._skipFocus = true;
            lf._dataView.refresh(true, values);
            lf._dataView._focus();
            lf._dataView._skipFocus = false;
        },
        _copyExternalLookupValues: function () {
            if (this.get_filterSource() && this.get_filterSource() != 'Context') {
                var master = this.get_master();
                if (master) {
                    var ditto = [];
                    var filter = this.get_externalFilter();
                    for (var i = 0; i < filter.length; i++) {
                        var f = this.findField(filter[i].Name);
                        if (f && !isNullOrEmpty(f.Copy)) {
                            master._copyLookupValues(master.get_currentRow(), f, null, ditto);
                        }
                    }
                    this._ditto = ditto;
                }
            }
        },
        _processSelectedLookupValues: function () {
            var values = [];
            var labels = [];
            var lf = this.get_lookupField();
            var dataValueField = this.findField(lf.ItemsDataValueField);
            var dataTextField = this.findField(lf.ItemsDataTextField);
            var r = this.get_selectedRow();
            if (!dataValueField) {
                for (var i = 0; i < this._allFields.length; i++) {
                    if (this._allFields[i].IsPrimaryKey)
                        Array.add(values, r[this._allFields[i].Index]);
                }
            }
            else
                Array.add(values, r[dataValueField.Index]);
            if (!dataTextField) {
                for (i = 0; i < this.get_fields().length; i++) {
                    f = this.get_fields()[i];
                    if (!f.Hidden && f.Type == 'String') {
                        Array.add(labels, f.format(r[f.AliasIndex]));
                        break;
                    }
                }
                if (labels.length == 0) {
                    for (i = 0; i < values.length; i++) {
                        var f = this.get_fields()[i];
                        if (!f.Hidden) {
                            Array.add(labels, f.format(r[f.AliasIndex]));
                            break;
                        }
                    }
                }
            }
            else
                Array.add(labels, dataTextField.format(r[dataTextField.Index]));
            this._copyLookupValues(r, lf);
            lf._dataView.changeLookupValue(lf.Index, values.length == 1 ? values[0] : values, labels.join(';'));
        },
        _showModal: function (args) {
            var that = this,
                tag = that.get_tag(),
                lastViewId = that.get_lastViewId(),
                commandName = args.commandName,
                commandArgument = args.commandArgument;
            that.set_lastCommandName(null);
            that.set_lastCommandArgument(null);
            that._render();
            if (args.commandName == 'Duplicate')
                args.commandName = 'New';
            if (_touch && that.get_isGrid()) {
                if ((commandName == 'Edit' || commandName == 'New') && (!commandName || commandArgument == lastViewId))
                    tag = (tag || '') + ' view-type-inline-editor';
            }
            var dataView = _app.showModal(this, that.get_controller(), commandArgument, commandName, commandArgument, that.get_baseUrl(), that.get_servicePath(),
                that.get_hasParent() || that._lookupInfo ? that._externalFilter : null,
                {
                    'filter': that.get_selectedKeyFilter(), 'ditto': that.get_ditto(), 'lastViewId': lastViewId, /*'transaction': that.get_transaction(),*/ 'filterSource': that.get_filterSource(), 'filterFields': that.get_filterFields(), 'showSearchBar': that.get_showSearchBar(),
                    'tag': tag, 'showActionButtons': that.get_showActionButtons()
                });
            that.set_ditto(null);
            that._savePosition();
            if (dataView && !dataView.inserting())
                dataView._position = that._position;
            that._restorePosition();
        },
        _savePosition: function () {
            if (!this.get_isForm()/*this.get_view().Type != 'Form'*/ && this._selectedRowIndex != null) {
                this._position = {
                    index: this._pageSize * this._pageIndex + this._selectedRowIndex,
                    count: this._totalRowCount,
                    filter: this.get_filter(),
                    sortExpression: this.get_sortExpression(),
                    key: Array.clone(this._selectedKey),
                    keyFilter: this._selectedKeyFilter,
                    active: false
                };
            }
        },
        _restorePosition: function () {
            if (this._position) {
                this._selectedKey = this._position.key;
                this._selectedKeyFilter = this._position.keyFilter;
                this._position = null;
            }
        },
        _advance: function (delta) {
            if (this._isBusy || !this._position || (delta == -1 & this._position.index == 0 || delta == 1 && this._position.index == this._position.count - 1)) return;
            this._position.index += delta;
            this._position.changing = true;
            this._position.changed = true;
            this._loadPage();
            this._position.changing = false;
        },
        cancel: function () {
            var that = this;
            if (that._cancelCallback)
                that._cancelCallback(this)
            else
                if (that._closeInstantDetails()) { }
                else if (that.endModalState('Cancel')) return;
                else if (that.get_backOnCancel() || !isNullOrEmpty(that._replaceTriggerViewId)) {
                    that.goBack(false);
                    setTimeout(function () {
                        location.replace(location.href)
                    }, 500);
                }
                else {
                    var lastCommand = that._lastCommandName;
                    that.set_lastCommandName('Cancel');
                    that._pendingSelectedEvent = true;
                    if (that.get_isForm()/*that.get_view().Type == 'Form'*/ || that.inserting()) {
                        if (_touch)
                            location.replace(location.href);
                        else {
                            that._forceSync();
                            that.goToView(that._lastViewId);
                        }
                    }
                    else if (that._totalRowCount < 0)
                        that.goToPage(-1);
                    else {
                        that._clearDynamicItems();
                        that._render();
                        _body_performResize();
                    }
                }
        },
        _convertValuesToParameters: function (values) {
            var i = 0;
            while (i < values.length) {
                var fv = values[i];
                fv.ReadOnly = true;
                if (fv.Name.match('PrimaryKey'))
                    Array.removeAt(values, i);
                else {
                    fv.Name = 'Parameters_' + fv.Name;
                    i++;
                }
            }
        },
        _actionConfirmed: function (args) {
            var that = this,
                actionArgs = that._createArguments(args),
                argValues = actionArgs.Values,
                valid = that._validateFieldValues(argValues, args.causesValidation == null || args.causesValidation);
            if (valid) {
                var context = that.get_confirmContext(),
                    dataView = that.get_parentDataView(),
                    childDataView = _app.find(args.commandArgument);
                if (childDataView) {
                    $(argValues).each(function () {
                        this.Name = 'Parameters_' + this.Name;
                    });
                    childDataView._externalFilter = argValues;
                    childDataView.sync();
                }
                else if (context && dataView) {
                    var values = dataView._paramValues = actionArgs.Values;
                    //                var i = 0;
                    //                while (i < values.length) {
                    //                    var fv = values[i];
                    //                    fv.ReadOnly = true;
                    //                    if (fv.Name.match('PrimaryKey'))
                    //                        Array.removeAt(values, i);
                    //                    else {
                    //                        fv.Name = 'Parameters_' + fv.Name;
                    //                        i++;
                    //                    }
                    //                }
                    that._convertValuesToParameters(values);
                    dataView._confirmDataViewId = that.get_id();
                    dataView._lookupActionProcessing(false)
                    dataView.executeAction(context.scope, context.actionIndex, context.rowIndex, context.groupIndex, true, context.action);
                    dataView._lookupActionProcessing(true)
                }
                else if (that._survey)
                    that.cancel();
            }
        },
        executeCommand: function (args) {
            if (this._isBusy) return;
            var that = this,
                skipInvoke = !!this._skipInvoke,
                rules,
                commandName = args.commandName,
                commandArgument = args.commandArgument;
            if (!skipInvoke) {
                rules = new _businessRules(this);
                rules.before(args);
                if (rules.canceled()) {
                    rules.dispose();
                    this._valid = commandName == 'Calculate';
                    return this._valid;
                }
            }
            switch (commandName) {
                case 'Select':
                case '':
                    this.set_lastCommandName(commandName);
                    this.set_lastCommandArgument(commandArgument);
                    if (this.get_lookupField() && commandArgument == '') this._processSelectedLookupValues();
                    else {
                        if (!isBlank(commandArgument)) {
                            this._savePosition();
                            if (this.get_showModalForms() && this.get_isForm(commandArgument) /*this.get_viewType(args.commandArgument) == 'Form'*/)
                                this._showModal(args);
                            else
                                this.goToView(commandArgument);
                        }
                        else
                            this._render();
                        if (__designer())
                            __designer_notifySelected(this);
                    }
                    break;
                case 'BatchEdit':
                    this.batchEdit(commandArgument);
                    break;
                case 'Edit':
                case 'New':
                case 'Duplicate':
                    if (!_touch) {
                        that._allowModalAutoSize();
                        that._fixHeightOfRow(false);
                        if (commandName == 'Edit') that._savePosition(); else that._restorePosition();
                    }
                    if (commandName == 'Duplicate') {
                        var r = that.get_selectedRow();
                        if (r) {
                            var dv = []
                            for (i = 0; i < that._allFields.length; i++) {
                                var f = that._allFields[i];
                                if (!f.OnDemand)
                                    Array.add(dv, { 'name': f.Name, 'value': r[f.Index], duplicated: true });
                            }
                            that._ditto = dv;
                        }
                    }
                    if (_touch && commandName == 'Edit' && that.get_isGrid() && (!commandArgument || commandArgument == that.get_viewId()))
                        if (that.inlineEditing())
                            _touch.edit.activate();
                        else
                            $(document).trigger($.Event('inlineeditingmode.dataview.app', { dataView: that, inlineEditing: true, editor: true }));
                    else {
                        var fc = that._get_focusedCell(),
                            extension = that.extension();
                        if (commandName == 'New' || commandName == 'Duplicate')
                            if (_touch && that.get_isGrid() && (!commandArgument || commandArgument == that.get_viewId())) {
                                // do nothing for now
                                _touch.notify('Not implemented.');
                                return;
                            }
                            else {
                                if (isNullOrEmpty(commandArgument))
                                    commandArgument = args.commandArgument = that.get_viewId();
                                if (extension && extension.clearSelection)
                                    extension.clearSelection(true, commandName);
                                else
                                    that._forgetSelectedRow(false, fc);
                                if (commandName == 'New') that._copyExternalLookupValues();
                            }
                        var stateChanged = commandName == 'Edit' && commandArgument == that.get_viewId() && !that.editing();
                        that.set_lastCommandName(commandName);
                        that.set_lastCommandArgument(commandArgument);
                        that._clearDynamicItems();
                        if (!isBlank(commandArgument) && !stateChanged)
                            if (_touch || that.get_showModalForms() && that.get_isForm(commandArgument) && !that.get_isModal())
                                that._showModal(args);
                            else
                                that.goToView(commandArgument);
                        else {
                            if (extension && extension.stateChanged)
                                extension.stateChanged();
                            else {
                                if (that.get_isModal())
                                    that._container.style.height = '';
                                if (that.get_isDataSheet() && !that._pendingChars)
                                    that._startInputListenerOnCell(that._selectedRowIndex, fc == null ? 0 : fc.colIndex);
                                that._render();
                            }
                        }
                        if (__designer())
                            __designer_notifySelected(that);
                    }
                    break;
                case 'Navigate':
                    this.navigate(commandArgument);
                    break;
                case 'Cancel':
                    this.cancel();
                    break;
                case 'Confirm':
                    this._actionConfirmed(args);
                    break;
                case 'Back':
                    history.go(!isNullOrEmpty(commandArgument) ? parseInteger(commandArgument) : -1);
                    break;
                case 'Report':
                case 'ReportAsPdf':
                case 'ReportAsImage':
                case 'ReportAsExcel':
                case 'ReportAsWord':
                    this.executeReport(args);
                    break;
                case 'ExportCsv':
                case 'ExportRowset':
                case 'ExportRss':
                    this.executeExport(args);
                    break;
                case '_ViewDetails':
                    this._viewDetails(commandArgument);
                    break;
                case 'ClientScript':
                    closeHoverMonitorInstance();
                    eval(commandArgument);
                    break;
                case 'SelectModal':
                case 'EditModal':
                    this.set_lastCommandName(null);
                    this.set_lastCommandArgument(null);
                    this._render();
                    var modalCmd = commandName.match(/^(\w+)Modal$/);
                    //this.set_lastCommandName(modalCmd[1]);
                    //this.set_lastCommandArgument(args.commandArgument);
                    var modalArg = commandArgument.split(',');
                    var modalController = modalArg.length == 1 ? this.get_controller() : modalArg[0];
                    var modalView = modalArg.length == 1 ? commandArgument : modalArg[1];
                    var filter = [];
                    for (i = 0; i < this.get_selectedKey().length; i++)
                        Array.add(filter, { Name: this._keyFields[i].Name, Value: this.get_selectedKey()[i] });
                    var dataView = _app.showModal(this, modalController, modalView, modalCmd[1], modalView, this.get_baseUrl(), this.get_servicePath(), filter);
                    dataView._parentDataViewId = this.get_id();
                    break;
                case 'Import':
                    if (_touch)
                        _app.import('upload', { dataView: this, view: commandArgument });
                    else
                        this._showImport(commandArgument);
                    break;
                case 'DataSheet':
                    this.writeContext('GridType', 'DataSheet');
                    this._forceFocusDataSheet = true;
                    this.changeViewType('DataSheet');
                    this.refreshAndResize();
                    break;
                case 'Grid':
                    this.writeContext('GridType', 'Grid');
                    this.changeViewType('Grid');
                    this.refreshAndResize();
                    break;
                /*            case 'Open':
                this.drillIn();
                break;*/
                case 'Status':
                    this._changeStatus(args);
                    break;
                case 'Search':
                    this._executeSearch(args.path);
                    break;
                case 'None':
                    return true;
                default:
                    var view = null,
                        m = commandArgument.match(/^view:(.+)$/),
                        actionArgs;
                    if (commandName == 'Insert' && m) {
                        view = m[1];
                        Array.clear(this._selectedKey);
                        this.updateSummary();
                        this.set_lastCommandName('New');
                        this.set_lastCommandArgument(view);
                    }
                    actionArgs = this._createArguments(args, view);
                    this._valid = this._validateFieldValues(actionArgs.Values, args.causesValidation == null || args.causesValidation)
                    if (this._valid)
                        if (skipInvoke)
                            this._invokeArgs = this._valid ? actionArgs : this._validationError;
                        else
                            this._execute(actionArgs);
                    break;
            }
            if (!skipInvoke) {
                rules.after(args);
                rules.dispose();
                return !rules.canceled();
            }
        },
        _forgetSelectedRow: function (notify, focusedCell) {
            if (!focusedCell)
                focusedCell = this._get_focusedCell();
            if (this.get_isDataSheet()) {
                if (focusedCell)
                    focusedCell.colIndex = 0;
            }
            this._lastSelectedRowIndex = !this._ignoreSelectedKey && this._selectedKey.length > 0 && this._rowIsSelected(this._selectedRowIndex) ? this._selectedRowIndex : -1;
            this._ignoreSelectedKey = false;
            Array.clear(this._selectedKey);
            this.updateSummary();
            if (notify)
                this.raiseSelected();
        },
        _changeStatus: function (args) {
            if (args.causesValidation) {
                var values = this._collectFieldValues();
                if (args.values)
                    values = Array.clone(args.values);
                if (!this._validateFieldValues(values, true))
                    return;
            }
            var statusField = this.findField('Status');
            if (!statusField) return;
            if (this.editing()) {
                var statusElem = this._get('_Item', statusField.Index);
                if (!statusElem) {
                    statusElem = document.createElement('input');
                    statusElem.type = 'hidden';
                    statusElem.id = String.format('{0}_Item{1}', this._id, statusField.Index);
                    this._container.appendChild(statusElem);
                }
                statusElem.value = args.commandArgument;
            }
            else {
                var row = this.get_selectedRow();
                row[statusField.Index] = args.commandArgument;
            }
            this._updateVisibility();
        },
        _notifyDesigner: function (changed) {
            if (__designer() && changed) {
                var lastArgs = this._lastArgs;
                external.ExplorerNodeChanged(this.get_controller(), this.get_viewId(), lastArgs.CommandName, lastArgs.CommandArgument, _serializer.serialize(lastArgs.Values));
            }
        },
        goBack: function (changed) {
            _app._navigated = true;
            this._notifyDesigner(changed);
            var l = location;
            if (l.href.match(/_explorerNode=/))
                l.replace(l.href);
            else
                history.go(-1);
        },
        /*get_path: function () {
        var path = this.readContext('TreePath');
        if (!path) {
        path = [];
        Array.add(path, { 'text': Web.DataViewResources.Grid.RootNodeText, 'key': [], 'filter': [], 'quickFind': '' });
        this.writeContext('TreePath', path);
        }
        return path;
        },*/
        /*drillIn: function (index) {
        if (!this.get_isTree()) return;
        for (var i = 0; i < this._allFields.length; i++) {
        var recursiveField = this._allFields[i];
        if (recursiveField.ItemsDataController == this.get_controller()) {
        var path = this.get_path();
        if (!path)
        path = [];
        if (index != null) {
        var levelInfo = path[index];
        while (path.length - 1 > index)
        Array.removeAt(path, path.length - 1);
        this.set_selectedKeyFilter([]);
        this.set_quickFindText(levelInfo.quickFind);
        this.set_filter(levelInfo.filter);
        if (path.length == 0) {
        this.set_selectedKey([]);
        this.removeFromFilter(recursiveField);
        this.refreshData();
        }
        else {
        var key = levelInfo.key;
        this.applyFieldFilter(i, '=', key);
        this.set_selectedKey(key);
        this._syncKeyFilter();
        }
        this.raiseSelected();
        }
        else {
        var field = this._fields[0];
        var text = field.format(this.get_selectedRow()[field.Index]);
        levelInfo = path[path.length - 1];
        levelInfo.filter = this.get_filter();
        levelInfo.quickFind = this.get_quickFindText();
        Array.add(path, { 'text': text, 'key': this.get_selectedKey(), 'filter': [], 'quickFind': '' });
        this.set_filter([]);
        this.set_quickFindText(null);
        this.applyFieldFilter(i, '=', this.get_selectedKey());
        }
        this.writeContext('TreePath', path);
        break;
        }
        }
        },*/
        _viewDetails: function (fieldName) {
            var f = this.findField(fieldName);
            if (f) {
                var keyFieldName = f.Name;
                if (f.ItemsDataController == this.get_controller())
                    for (var i = 0; i < this._allFields.length; i++) {
                        if (this._allFields[i].IsPrimaryKey) {
                            keyFieldName = this._allFields[i].Name;
                            break;
                        }
                    }
                var contextFilter = this.get_contextFilter(f);
                if (__designer()) {
                    var link = String.format('{0}&{1}={2}', f.ItemsDataController, !isNullOrEmpty(f.ItemsDataValueField) ? f.ItemsDataValueField : keyFieldName, this.get_selectedRow()[f.Index]);
                    for (i = 0; i < contextFilter.length; i++) {
                        var filter = contextFilter[i];
                        link = String.format('{0}&{1}={2}', link, filter.Name, filter.Value);
                    }
                    link = this.resolveClientUrl(String.format('~/Details.{0}?l={1}', __designer() ? 'htm' : 'aspx', encodeURIComponent(link)));
                    location.href = link;
                    _app._navigated = false;
                }
                else {
                    filter = [{ 'Name': !isNullOrEmpty(f.ItemsDataValueField) ? f.ItemsDataValueField : keyFieldName, 'Value': this.fieldValue(f.Name) }];
                    Array.addRange(filter, contextFilter);
                    var dataView = _app.showModal(this, f.ItemsDataController, "editForm1", "Select", "editForm1", this.get_baseUrl(), this.get_servicePath(), filter, { useCase: 'ObjectRef' });
                    dataView.set_showSearchBar(this.get_showSearchBar());
                    dataView._parentDataViewId = this.get_id();
                    dataView._closeViewDetails = true;
                }

                //            for (i = 0; i < contextFilter.length; i++) {
                //                var filter = contextFilter[i];
                //                link = String.format('{0}&{1}={2}', link, filter.Name, filter.Value);

                //            }

                //            var link = String.format('{0}&{1}={2}', f.ItemsDataController, !isNullOrEmpty(f.ItemsDataValueField) ? f.ItemsDataValueField : keyFieldName, this.get_selectedRow()[f.Index]);
                //            var contextFilter = this.get_contextFilter(f);
                //            for (i = 0; i < contextFilter.length; i++) {
                //                var filter = contextFilter[i];
                //                link = String.format('{0}&{1}={2}', link, filter.Name, filter.Value);

                //            }



                //            link = this.resolveClientUrl(String.format('~/Details.{0}?l={1}', __designer() ? 'htm' : 'aspx', encodeURIComponent(link)));
                //            if (Web.DataViewResources.Lookup.ShowDetailsInPopup)
                //            //window.open(link, '_blank', 'scrollbars=yes,height=100,resizable=yes');
                //                this._navigate('_blank:' + link, 'scrollbars=yes,height=100,resizable=yes');
                //            else
                //                window.location.href = link;
                //            _app._navigated = false;
            }
        },
        changeViewType: function (type) {
            this.cancelDataSheet();
            if (!this._viewTypes)
                this._viewTypes = [];
            this._viewTypes[this.get_viewId()] = type;
            this._clearCache();
        },
        _parseLocation: function (url, row, values) {
            if (!row) row = this.get_selectedRow();
            if (!url) return null;
            url = this.resolveClientUrl(url);
            var iterator = /([\s\S]*?)\{(\w+)?\}/g;
            var formattedurl = '';
            var lastIndex = -1;
            var match = iterator.exec(url);
            while (match) {
                formattedurl += match[1];
                if (values && this._lastArgs) {
                    for (var i = 0; i < values.length; i++) {
                        var v = values[i];
                        if (v.Name == match[2]) {
                            formattedurl += this._lastArgs.CommandName.match(/Insert/i) ? v.NewValue : v.OldValue;
                            break;
                        }
                    }
                }
                else {
                    var field = match[2].match(/^\d+$/) ? this.get_fields()[parseInteger(match[2])] : this.findField(match[2]);
                    if (field) {
                        var fv = row[field.Index];
                        if (fv != null)
                            formattedurl += match.index == 0 ? fv : encodeURIComponent(fv);
                    }
                }
                lastIndex = iterator.lastIndex;
                match = iterator.exec(url);
            }
            if (lastIndex != -1) url = formattedurl + (lastIndex < url.length ? url.substr(lastIndex) : '');
            if (url.startsWith('?'))
                url = location.pathname + url;
            return url;
        },
        _parseText: function (text, row) {
            var that = this;
            if (!row) row = that.get_selectedRow();
            if (!text) return null;
            var parent = that.get_parentDataView(that),
                parentSelectedKeyList = parent._selectedKeyList,
                selectedCount = parentSelectedKeyList ? parentSelectedKeyList.length : 0;
            if (!selectedCount && parent._selectedKey != null && parent._selectedKey.length)
                selectedCount++;
            if (parentSelectedKeyList && Array.indexOf(parentSelectedKeyList, 'null') != -1)
                selectedCount--;

            text = text.replace(/\{\$count\}/gi, parent._totalRowCount);
            text = text.replace(/\{\$selected\}/gi, selectedCount);
            var iterator = /([\s\S]*?)\{(\w+)?\}/g,
                formattedText = '',
                lastIndex = -1,
                match = iterator.exec(text);
            while (match) {
                formattedText += match[1];
                var field = match[2].match(/^\d+$/) ? that.get_fields()[parseInteger(match[2])] : that.findField(match[2]);
                if (field) {
                    field = that._allFields[field.AliasIndex];
                    var fv = row[field.Index];
                    if (fv != null) {
                        fv = field.format(fv);
                        //fv = fv.replace(/\'/g, '\\\'');
                        formattedText += fv;
                    }
                }
                lastIndex = iterator.lastIndex;
                match = iterator.exec(text);
            }
            if (lastIndex != -1) text = formattedText + (lastIndex < text.length ? text.substr(lastIndex) : '');
            return text;
        },
        _showImport: function (view) {
            if (!view) view = this.get_viewId();
            this._importView = view;
            this._importElement = document.createElement('div');
            this._importElement.id = this.get_id() + '$Import';
            this._importElement.className = 'Import';
            var sb = new Sys.StringBuilder();
            var s = String.format('<a href="javascript:" onclick="$find(\'{0}\')._downloadImportTemplate(\'{1}\');return false;">{2}</a>', this.get_id(), view, resourcesData.Import.DownloadTemplate);
            sb.appendFormat('<div id="{0}$ImportStatus" class="Status"><span>{1}</span> {2}</div>', this.get_id(), resourcesData.Import.UploadInstruction, s);
            sb.appendFormat('<div id="{0}$ImportMap" class="Map" style="display:none"></div>', this.get_id());
            sb.appendFormat('<iframe src="{1}?parentId={0}&controller={2}&view={3}" frameborder="0" scrolling="no" id="{0}$ImportFrame" class="Import"></iframe>', this.get_id(), this.resolveClientUrl(this.get_appRootPath() + 'Import.ashx'), this.get_controller(), view);
            sb.appendFormat('<div class="Email">{1}:<br/><input type="text" id="{0}$ImportEmail"/></div>', this.get_id(), resourcesData.Import.Email);
            sb.appendFormat('<div class="Buttons"><button id="{0}$StartImport" onclick="$find(\'{0}\')._startImportProcessing();return false" style="display:none">{1}</button><button id="{0}$CancelImport" onclick="$find(\'{0}\')._closeImport();return false">{2}</button></div>', this.get_id(), resourcesData.Import.StartButton, resourcesModalPopup.CancelButton);
            this._importElement.innerHTML = sb.toLocaleString();
            //document.body.appendChild(this._importElement);
            this._appendModalPanel(this._importElement);
            this._importPopup = $create(AjaxControlToolkit.ModalPopupBehavior, { 'id': this.get_id() + '$ImportPopup', PopupControlID: this._importElement.id, DropShadow: true, BackgroundCssClass: 'ModalBackground' }, null, null, this._container.getElementsByTagName('a')[0]);
            this._saveTabIndexes();
            this._importPopup.show();
        },
        _closeImport: function () {
            this._disposeImport();
            this._restoreTabIndexes();
        },
        _disposeImport: function () {
            if (this._importElement) {
                this._importPopup.hide();
                this._importPopup.dispose();
                this._importPopup = null;
                this._importElement.parentNode.removeChild(this._importElement);
                delete this._importElement;
            }
        },
        _disposePendingUploads: function () {
            $(this._pendingUploads).each(function () {
                this.files = null;
                this.form = null;
            });
            this._pendingUploads = null;
            $(this._container).find('.app-drop-box').each(function () {
                _app.upload('destroy', { container: this });
            });
        },
        _downloadImportTemplate: function (view) {
            this.executeExport({ commandName: 'ExportTemplate', commandArgument: String.format('{0},{1}', this.get_controller(), view) });
        },
        _initImportUpload: function (frameDocument) {
            var div = frameDocument.createElement('div');
            div.innerHTML = String.format('<form method="post" enctype="multipart/form-data"><input type="file" id="ImportFile" name="ImportFile" style="font-size:8.5pt;font-family:tahoma;padding:2px 0px 4px 0px;" onchange="parent.window.$find(\'{0}\')._startImportUpload(this.value);this.parentNode.submit()"/></form>', this.get_id());
            frameDocument.body.appendChild(div);
            //frameDocument.getElementById('ImportFile').focus();
            Sys.UI.DomElement.setFocus(frameDocument.getElementById('ImportFile'));
        },
        _get_importStatus: function () {
            return $get(this.get_id() + '$ImportStatus');
        },
        _get_importFrame: function () {
            return $get(this.get_id() + '$ImportFrame');
        },
        _startImportUpload: function (fileName) {
            var parts = fileName.split(/\\/);
            this._importFileName = parts[parts.length - 1];
            Sys.UI.DomElement.setVisible(this._get_importFrame(), false);
            $(this._get_importStatus()).addClass('Wait');
            this._get_importStatus().innerHTML = resourcesData.Import.Uploading;
        },
        _finishImportUpload: function (frameDocument) {
            Sys.UI.DomElement.removeCssClass(this._get_importStatus(), 'Wait');
            Sys.UI.DomElement.addCssClass(this._get_importStatus(), 'Ready');
            var errors = frameDocument.getElementById('Errors');
            if (errors)
                this._get_importStatus().innerHTML = errors.value;
            else {
                var fileName = frameDocument.getElementById('FileName');
                var numberOfRecords = frameDocument.getElementById('NumberOfRecords');
                var availableImportFields = frameDocument.getElementById('AvailableImportFields').value.trim().split(/\r?\n/);
                var fieldMap = frameDocument.getElementById('FieldMap').value.trim().split(/\r?\n/);
                this._get_importStatus().innerHTML = String.format(resourcesData.Import.MappingInstruction, numberOfRecords.value, this._importFileName);
                this._importFileName = frameDocument.getElementById('FileName').value;
                var importButton = $get(this.get_id() + '$StartImport');
                Sys.UI.DomElement.setVisible(importButton, true);
                //importButton.focus();
                Sys.UI.DomElement.setFocus(importButton);
                var sb = new Sys.StringBuilder();
                sb.append('<table>');
                for (var i = 0; i < fieldMap.length; i++) {
                    var mapping = fieldMap[i].match(/^(.+?)=(.+?)?$/);
                    sb.appendFormat('<tr><td>{2}</td><td><select id="{0}$ImportField{1}"><option value="">{3}</option>', this.get_id(), i, _app.htmlEncode(mapping[1]), resourcesData.Import.AutoDetect);
                    for (var j = 0; j < availableImportFields.length; j++) {
                        var option = availableImportFields[j].split('=');
                        if (option[0] == mapping[2]) {
                            sb.appendFormat('<option value="{0}"', option[0]);
                            sb.append(' selected="selected"');
                            sb.appendFormat('>{0}</option>', _app.htmlEncode(option[1]));
                        }
                    }
                    sb.append('</select></td></tr>');
                }
                sb.append('</table>');
                var importMapElem = $get(this.get_id() + '$ImportMap');
                Sys.UI.DomElement.setVisible(importMapElem, true);
                importMapElem.innerHTML = sb.toString();
                var $importMap = $(importMapElem).find('select').change(function () {
                    refreshOptions();
                });
                var refreshOptions = function () {
                    var unmappedFields = Array.clone(fieldMap);
                    var mappedFields = [];
                    var maxWidth = 0;
                    $importMap.each(function () {
                        $(this).css('width', 'auto');
                        if (this.value != '')
                            Array.add(mappedFields, this.value);
                    }).each(function () {
                        var that = this;
                        $(that).find('option').filter(function () {
                            return that.value != this.value && this.value != '';
                        }).remove();
                        for (var j = 0; j < availableImportFields.length; j++) {
                            var option = availableImportFields[j].split('=');
                            if (!Array.contains(mappedFields, option[0])) {
                                $(this).append($('<option>', { value: option[0] }).text(option[1]));
                            }
                        }
                    }).each(function () {
                        var w = $(this).outerWidth();
                        if (w > maxWidth)
                            maxWidth = w;
                    }).css('width', maxWidth);
                };
                refreshOptions();
            }
            this._importPopup.show();
            this._get_importFrame().parentNode.removeChild(this._get_importFrame());
        },
        _startImportProcessing: function () {
            var emailElem = $get(this.get_id() + '$ImportEmail');
            var email = emailElem.value.replace(/;/g, ',');
            if (isBlank(email) && !confirm(resourcesData.Import.EmailNotSpecified)) {
                //emailElem.focus();
                Sys.UI.DomElement.setFocus(emailElem);
                return;
            }
            var sb = new Sys.StringBuilder();
            sb.appendFormat('{0};{1};{2};{3};', this._importFileName, this.get_controller(), this._importView, email);
            var i = 0;
            while (true) {
                var mapping = $get(this.get_id() + '$ImportField' + i);
                if (!mapping) break;
                sb.append(mapping.value);
                sb.append(';');
                i++;
            }
            this.executeCommand({ commandName: 'ProcessImportFile', commandArgument: sb.toString() });
            this._closeImport();
            alert(resourcesData.Import.Processing);
            this.refresh();
        },
        navigate: function (location, values) {
            var targetView;
            this.set_selectedValue(this.get_selectedKey());
            closeHoverMonitorInstance();
            location = this._parseLocation(location, null, values);
            for (var i = 0; i < this.get_views().length; i++)
                if (this.get_views()[i].Id == location) {
                    targetView = this.get_views()[i];
                    break;
                }
            if (targetView)
                this.goToView(location);
            else
                this._navigate(location);
        },
        _navigate: function (location, features) {
            _app._navigated = true;
            var m = location.match(_app.LocationRegex);
            if (typeof __dauh != 'undefined')
                if (m)
                    this.encodePermalink(m[2], m[1], features)
                else
                    this.encodePermalink(location);
            else
                if (m) {
                    _app._navigated = false;
                    open(m[2], m[1], features ? features : '');
                }
                else
                    window.location.href = location;
        },
        get_contextFilter: function (field, values) {
            var contextFilter = [];
            if (!isNullOrEmpty(field.ContextFields)) {
                var contextValues = values ? values : this._collectFieldValues(true);
                var iterator = /(\w+)(\s*=\s*(.+?)){0,1}\s*(,|$)/g;
                var m = iterator.exec(field.ContextFields);
                while (m) {
                    var n = !isNullOrEmpty(m[3]) ? m[3] : m[1];
                    var m2 = n.match(/^\'(.+)\'$/);
                    if (!m2)
                        m2 = n.match(/^(\d+)$/);
                    if (m2) {
                        for (var i = 0; i < contextFilter.length; i++) {
                            if (contextFilter[i].Name == m[1]) {
                                contextFilter[i].Value += '\0=' + m2[1];
                                m2 = null;
                                break;
                            }
                        }
                        if (m2) Array.add(contextFilter, { Name: m[1], Value: m2[1], Literal: true });
                    }
                    else {
                        var f = this.findField(n);
                        if (f) {
                            for (i = 0; i < contextValues.length; i++) {
                                if (contextValues[i].Name == f.Name) {
                                    var v = contextValues[i];
                                    var fieldValue = v.Modified ? v.NewValue : v.OldValue;
                                    Array.add(contextFilter, { Name: m[1], Value: fieldValue });
                                    break;
                                }
                            }
                        }
                    }
                    m = iterator.exec(field.ContextFields);
                }
            }
            return contextFilter;
        },
        showLookup: function (fieldIndex) {
            if (!this.get_enabled()) return;
            var field = this._allFields[fieldIndex];
            if (!field._lookupModalBehavior) {
                var showLink = $get(this.get_id() + '_Item' + field.Index + '_ShowLookupLink');
                if (showLink) {
                    var panel = field._lookupModalPanel = document.createElement('div');
                    //document.body.appendChild(panel);
                    this._appendModalPanel(panel);
                    panel.className = 'ModalPanel';
                    panel.id = this.get_id() + '_ItemLookupPanel' + field.Index;
                    panel.innerHTML = String.format('<table style="width:100%;height:100%"><tr><td valign="middle" align="center"><table cellpadding="0" cellspacing="0"><tr><td class="ModalTop"><div style="height:1px;font-size:1px"></div></td><td><div style="height:1px;font-size:1px"></div></td></tr><tr><td align="left" valign="top" id="{0}_ItemLookupPlaceholder{1}"  class="ModalPlaceholder"></td><td class="RightSideShadow"></td></tr><tr><td colspan="2"><div class="BottomShadow"></div></td></tr></table></td></tr></table>', this.get_id(), field.Index);
                    field._lookupModalBehavior = $create(AjaxControlToolkit.ModalPopupBehavior, { id: this.get_id() + "_ItemLookup" + field.Index, PopupControlID: panel.id, BackgroundCssClass: 'ModalBackground' }, null, null, showLink);
                }
            }
            else
                field._lookupDataControllerBehavior._render();
            var contextFilter = this.get_contextFilter(field);
            var focusQF = true;
            if (!field._lookupDataControllerBehavior) {
                focusQF = false;
                field._lookupDataControllerBehavior = $create(Web.DataView, {
                    id: this.get_id() + '_LookupView' + fieldIndex, baseUrl: this.get_baseUrl(), pageSize: field.ItemsPageSize ? field.ItemsPageSize : resourcesPager.PageSizes[0], showFirstLetters: field.ItemsLetters, servicePath: this.get_servicePath(),
                    controller: field.ItemsDataController, viewId: field.ItemsDataView, showActionBar: resources.Lookup.ShowActionBar, lookupField: field, externalFilter: contextFilter, filterSource: contextFilter.length > 0 ? 'Context' : null, 'showSearchBar': this.get_showSearchBar(), 'searchOnStart': this.get_showSearchBar() && field.SearchOnStart, 'description': field.ItemsDescription
                }, null, null, $get(this.get_id() + '_ItemLookupPlaceholder' + field.Index));
            }
            else if (contextFilter.length > 0) {
                field._lookupDataControllerBehavior.set_externalFilter(contextFilter);
                field._lookupDataControllerBehavior.goToPage(-1);
                focusQF = true;
            }
            this._saveTabIndexes();
            field._lookupModalBehavior.show();
            if (focusQF) field._lookupDataControllerBehavior._focusQuickFind(true);
            $addHandler(document.body, 'keydown', field._lookupDataControllerBehavior._bodyKeydownHandler);
            field._lookupDataControllerBehavior._adjustLookupSize();
            this._lookupIsActive = true;
        },
        changeLookupValue: function (fieldIndex, value, text) {
            var field = this._allFields[fieldIndex],
                that = this;
            that._focusedFieldName = field.Name;
            setTimeout(function () {
                that._restoreTabIndexes();
                var itemId = that.get_id() + '_Item' + fieldIndex;
                var itemTextId = that.get_id() + '_Item' + field.AliasIndex;
                Sys.UI.DomElement.setVisible($get(itemId + '_ClearLookupLink'), true);
                var elem = $get(itemId + '_ShowLookupLink');
                elem.innerHTML = that.htmlEncode(field, text);
                Sys.UI.DomElement.setFocus(elem);
                //$get(itemId + '_ShowLookupLink').focus();
                if (value)
                    value = field.format(value);
                $get(itemId).value = value;
                if (itemId != itemTextId) $get(itemTextId).value = text;
                $(elem).closest('table').css('width', '');
                //while (elem.tagName != 'TABLE') elem = elem.parentNode;
                //elem.style.width = '';
                that._updateLookupInfo(value, text);
                that._valueChanged(fieldIndex);
                that._adjustModalHeight();
            }, 0);
            that._closeLookup(field);
        },
        clearLookupValue: function (fieldIndex) {
            var field = this._allFields[fieldIndex];
            var itemId = this.get_id() + '_Item' + fieldIndex;
            var itemTextId = this.get_id() + '_Item' + field.AliasIndex;
            Sys.UI.DomElement.setVisible($get(itemId + '_ClearLookupLink'), false);
            $get(itemId + '_ShowLookupLink').innerHTML = resources.Lookup.SelectLink;
            $get(itemId).value = '';
            $get(itemTextId).value = '';
            if (!isNullOrEmpty(field.Copy)) {
                var values = [];
                var iterator = /(\w+)=(\w+)/g;
                var m = iterator.exec(field.Copy);
                while (m) {
                    if (m[2] == 'null')
                        Array.add(values, { 'name': m[1], 'value': null });
                    m = iterator.exec(field.Copy);
                }
                if (values.length > 0)
                    this.refresh(true, values);
            }

            this._updateLookupInfo('', resources.Lookup.SelectLink);
            $get(itemId + '_ShowLookupLink').focus();
            this._valueChanged(fieldIndex);
            this._adjustModalHeight();
        },
        _updateLookupInfo: function (value, text) {
            var lookupText = $get(this.get_id() + '_Text0');
            if (lookupText) {
                lookupText.value = text;
                lookupText.name = lookupText.id;
                var lookupValue = $get(this.get_id() + '_Item0');
                lookupValue.value = value;
                lookupValue.name = lookupValue.id;
                if (this.get_lookupPostBackExpression()) {
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    if (prm)
                        eval("Sys.WebForms.PageRequestManager.getInstance()._doPostBack" + this.get_lookupPostBackExpression().match(/\w+(.+)/)[1]);
                    else
                        eval(this.get_lookupPostBackExpression());
                }
            }
        },
        createNewLookupValue: function (fieldIndex) {
            var field = this._newLookupValueField = this._allFields[fieldIndex];
            var cnv = this._createNewView = _app.showModal(/*$get(String.format('{0}_Item{1}_CreateNewLookupLink', this.get_id(), field.Index))*/this, field.ItemsDataController, field.ItemsNewDataView, 'New', field.ItemsNewDataView, this.get_baseUrl(), this.get_servicePath(), this.get_contextFilter(field));
            cnv._parentDataViewId = this.get_id();
            cnv.add_executed(Function.createDelegate(this, this._saveNewLookupValueCompleted));
            cnv.set_showSearchBar(this.get_showSearchBar());
            this._lookupIsActive = true;
        },
        _saveNewLookupValueCompleted: function (sender, args) {
            if (args.result.Errors.length > 0) return;
            args.handled = true;
            _app.hideMessage();
            var v = null;
            var newLookupValueField = this._newLookupValueField;
            var lookupDataValueField = newLookupValueField.ItemsDataValueField;
            if (isNullOrEmpty(lookupDataValueField))
                lookupDataValueField = sender._keyFields[0].Name;
            if (args.result.Values.length == 0) args.result.Values = sender._lastArgs.Values;
            for (var j = 0; j < args.result.Values.length; j++)
                if (args.result.Values[j].Name == lookupDataValueField) {
                    v = args.result.Values[j].NewValue;
                    break;
                }
            var t = null;
            var lookupDataTextField = newLookupValueField.ItemsDataTextField;
            if (isNullOrEmpty(lookupDataTextField))
                lookupDataTextField = sender._fields[0].Name;
            for (i = 0; i < sender._lastArgs.Values.length; i++) {
                if (sender._lastArgs.Values[i].Name == lookupDataTextField) {
                    t = sender._lastArgs.Values[i].NewValue;
                    break;
                }
            }
            this._createNewView.endModalState('Cancel');
            this._copyLookupValues(null, newLookupValueField, sender._lastArgs.Values);
            this.changeLookupValue(newLookupValueField.Index, v, t);
            this._lookupIsActive = false;
        },
        hideLookup: function (fieldIndex) {
            var field = fieldIndex ? this._allFields[fieldIndex] : this.get_lookupField();
            var dv = this.get_lookupField()._dataView;
            dv._closeLookup(field);
            dv._restoreTabIndexes();
            $get(dv.get_id() + '_Item' + field.Index + '_ShowLookupLink').focus();
        },
        closeLookupAndCreateNew: function () {
            this.hideLookup();
            var field = this.get_lookupField();
            field._dataView.createNewLookupValue(field.Index);
        },
        htmlEncode: function (field, s) { var f = this._allFields[field.AliasIndex]; return f.HtmlEncode ? (f.Type == 'String' ? _app.htmlEncode(s) : s) : s; },
        filterIsExternal: function (fieldName) {
            if (this._externalFilter.length == 0) return false;
            for (var i = 0; i < this._filter.length; i++) {
                var name = this._filter[i].match(/(\w+):/)[1];
                var found = false;
                if (!fieldName || fieldName == name)
                    for (var j = 0; j < this._externalFilter.length; j++)
                        if (this._externalFilter[j].Name == name) {
                            found = true;
                            break;
                        }
                if (!found) return false;
            }
            return true;
        },
        updateSummary: function () {
            if (!this.get_showInSummary() || _touch) return;
            var summaryBox = null;
            if (!this._summaryId) {
                var sideBar = $getSideBar();
                if (!sideBar) this._summaryId = '';
                else {
                    this._summaryId = 'PageSummary_' + this.get_id();
                    summaryBox = $get('PageSummaryBox');
                    if (!summaryBox) {
                        summaryBox = document.createElement('div');
                        summaryBox.id = 'PageSummaryBox';
                        summaryBox.className = 'TaskBox Summary';
                        summaryBox.innerHTML = String.format('<div class="Inner"><div class="Summary">{0}</div></div>', resources.Menu.Summary);
                        sideBar.insertBefore(summaryBox, sideBar.childNodes[sideBar._hasActivators ? 1 : 0]);
                        summaryBox._numberOfVisibleSummaries = 0;
                    }
                    var viewSummary = $get(this._summaryId);
                    if (!viewSummary) {
                        viewSummary = document.createElement('div');
                        viewSummary.id = this._summaryId;
                        summaryBox.childNodes[0].appendChild(viewSummary);
                    }
                }
            }
            if (this._summaryId) {
                if (!summaryBox) summaryBox = $get('PageSummaryBox');
                if (!this._rows || this._rows.length == 0) {
                    if (!this._filterSource)
                        Sys.UI.DomElement.setVisible(summaryBox, false);
                    return;
                }
                var row = this.get_selectedRow();
                viewSummary = $get(this._summaryId);
                var sb = new Sys.StringBuilder();
                var saveLastCommandName = this._lastCommandName;
                var saveViewType = this.get_view().Type;
                this.get_view().Type = 'Grid';
                this._lastCommandName = null;
                var empty = true;
                if (this._selectedKey.length > 0) {
                    var first = true;
                    var count = 0;
                    for (var i = 0; i < this._allFields.length; i++) {
                        var f = this._allFields[i];
                        if (f.ShowInSummary/* && !f.Hidden*/) {
                            empty = false;
                            sb.append('<div class="Field">');
                            if (first)
                                first = false;
                            else
                                sb.append('<div class="Divider"></div>');
                            sb.appendFormat('<div class="Label">{0}</div>', String.trimLongWords(this._allFields[f.AliasIndex].Label));
                            sb.append('<div class="Value">');
                            this._renderItem(sb, f, row, false, false, false, false, true);
                            sb.append('</div></div>');
                            count++;
                            if (this.get_summaryFieldCount() > 0 && count >= this.get_summaryFieldCount()) break;
                        }
                    }
                }
                Sys.UI.DomElement.setVisible(viewSummary, !empty);
                if (empty && this._summaryIsVisible)
                    summaryBox._numberOfVisibleSummaries--;
                else if (!empty && !this._summaryIsVisible || !empty && this._summaryIsVisible == null)
                    summaryBox._numberOfVisibleSummaries++;
                this._summaryIsVisible = !empty;
                Sys.UI.DomElement.setVisible(summaryBox, summaryBox._numberOfVisibleSummaries > 0);
                var s = sb.toString();
                viewSummary.innerHTML = s;
                var clearPermalink = this._lastArgs != this._lastClearArgs && this._lastArgs.CommandName == 'Delete';
                if (!_touch && (!empty || clearPermalink) && this.get_filterSource() == null && typeof (Web.Membership) != "undefined") {
                    if (clearPermalink) this._lastClearArgs = this._lastArgs;
                    Web.Membership._instance.addPermalink(String.format('{0}&_controller={1}&_commandName=Select&_commandArgument=editForm1', this.get_keyRef(), this.get_controller()), clearPermalink ? null : String.format('<div class="TaskBox" style="width:{2}px"><div class="Inner"><div class="Summary">{0}</div>{1}</div></div>', document.title, s, viewSummary.offsetWidth == 0 ? 135 : viewSummary.offsetWidth));
                }
                sb.clear();
                this._lastCommandName = saveLastCommandName;
                this.get_view().Type = saveViewType;
            }
        },
        hasDetails: function () {
            var that = this,
                result = that._hasDetails;
            if (!result)
                $(that._allFields).each(function () {
                    if (this.Type == 'DataView') {
                        result = true;
                        return false;
                    }
                });
            return !that.odp && (!!result && !that._doneCallback);
        },
        get_hasDetails: function () {
            return !!this._hasDetails;
        },
        get_hasParent: function () {
            return this._hasParent == true;
        },
        //get_usesTransaction: function () {
        //    return this._usesTransaction == true;
        //},
        //get_inTransaction: function () {
        //    return this.get_transaction() != null;
        //},
        add_selected: function (handler) {
            var that = this;
            if (!that._dataViewFieldName)
                that._hasDetails = true;
            that.get_events().addHandler('selected', handler);
        },
        remove_selected: function (handler) {
            this.get_events().removeHandler('selected', handler);
        },
        raiseSelected: function (eventArgs) {
            if (_app._navigated) return;
            var pendingEdit = _touch ? _touch.edit._pending : null;
            if (pendingEdit && !(pendingEdit.direction == 'enter' || !pendingEdit.direction))
                return; // the selection will change - do not refresh.
            var handler = this.get_events().getHandler('selected');
            if (handler) handler(this, Sys.EventArgs.Empty);
            if (!this.multiSelect())
                this.set_selectedValue(this.get_selectedKey());
        },
        add_executed: function (handler) {
            this.get_events().addHandler('executed', handler);
        },
        remove_executed: function (handler) {
            this.get_events().removeHandler('executed', handler);
        },
        raiseExecuted: function (eventArgs) {
            var handler = this.get_events().getHandler('executed');
            if (handler) handler(this, eventArgs);
        },
        _closeLookup: function (field) {
            $closeHovers();
            if (field && field._lookupModalBehavior) {
                field._lookupModalBehavior.hide();
                $removeHandler(document.body, 'keydown', field._lookupDataControllerBehavior._bodyKeydownHandler);
            }
            this._lookupIsActive = false;
            if (window.event) {
                var ev = new Sys.UI.DomEvent(event);
                ev.stopPropagation();
                ev.preventDefault();
            }
        },
        _collectFieldValues: function (all) {
            //if (all == null) all = false;
            all = true;
            var values,
                selectedRow,
                inserting,
                extension = this.extension();
            if (extension && extension.collect)
                return extension.collect();
            inserting = this.inserting();
            values = new Array();
            selectedRow = this.get_selectedRow();
            if (!selectedRow && !inserting) return values;
            for (var i = 0; i < this._allFields.length; i++) {
                var field = this._allFields[i],
                    isRadioButtonList = field.ItemsStyle == 'RadioButtonList',
                    element = this._get('_Item', i);
                if (field.ReadOnly && !all)
                    element = null;
                else if (isRadioButtonList) {
                    var j = 0,
                        option = $get(this.get_id() + '_Item' + i + '_' + j);
                    while (option) {
                        if (option.checked) {
                            element = option;
                            break;
                        }
                        else
                            element = null;
                        j++;
                        option = $get(this.get_id() + '_Item' + i + '_' + j);
                    }
                }
                else if (field.ItemsStyle == 'CheckBoxList' && element) {
                    j = 0;
                    option = $get(this.get_id() + '_Item' + i + '_' + j);
                    if (option) {
                        element.value = '';
                        while (option) {
                            if (option.checked) {
                                if (element.value.length > 0) element.value += ',';
                                element.value += option.value;
                            }
                            j++;
                            option = $get(this.get_id() + '_Item' + i + '_' + j);
                        }
                    }
                }
                if (field.ClientEditor && element) {
                    if (field.ClientEditor == 'Web$DataView$RichText') {
                        var factory = _app.EditorFactories[field.ClientEditor.replace('.', '$')]
                        if (factory.persist)
                            factory.persist(element);
                    }
                }
                else if (field.Editor && element) {
                    var frame = $get(element.id + '$Frame');
                    if (frame) {
                        editor = _app.Editors[field.EditorId];
                        if (editor)
                            element.value = editor.GetValue();

                    }
                }
                var elementValue = element ? element.value : null;
                if (elementValue)
                    if (field.Type.startsWith('Date')) {
                        var d = Date.tryParseFuzzyDate(elementValue, field.DataFormatString);
                        if (d != null && element.type == 'text')
                            elementValue = element.value = field.DateFmtStr ? String.format(field.DateFmtStr, d) : field.format(d);
                    }
                    else if (!isBlank(field.DataFormatString) && field.isNumber()) {
                        /* == '{0:c}' ir -- '{0:p} */
                        var n = Number.tryParse(elementValue);
                        if (n != null) {
                            if (field.DataFormatString.match(/\{0:p\}/i) && n > 1)
                                n = n / 100;
                            elementValue = element.value = field.format(n);
                        }
                    }
                if (field.TimeFmtStr) {
                    var timeElem = this._get('_Item$Time', i)
                    if (timeElem) {
                        d = Date.tryParseFuzzyTime(timeElem.value);
                        if (d != null) {
                            timeElem.value = String.localeFormat(field.TimeFmtStr, d);
                            elementValue += ' ' + timeElem.value;
                        }
                    }
                }
                if (!field.OnDemand && (element || field.IsPrimaryKey || (!field.ReadOnly || all))) {
                    var add = true;
                    var readOnly = false;
                    if (this._lastCommandName == 'BatchEdit') {
                        var cb = $get(String.format('{0}$BatchSelect{1}', this.get_id(), field.Index));
                        readOnly = field.TextMode == 4 || field.Hidden || field.ReadOnly;
                        add = field.IsPrimaryKey || readOnly || cb && cb.checked;
                    }
                    if (add) Array.add(values,
                        {
                            Name: field.Name, OldValue: inserting ? (/*this._newRow ? this._newRow[field.Index] : */null) : selectedRow[field.Index],
                            NewValue: element && elementValue != null ? (field.Type == 'Boolean' && elementValue.length > 0 ? elementValue == 'true' : (elementValue.length == 0 ? null : elementValue)) : (!inserting && (field.TextMode == 4 || field.Hidden || field.ReadOnly) ? selectedRow[field.Index] : null),
                            Modified: (element != null && !(!inserting && field.Type == 'String' && isNullOrEmpty(elementValue) && isNullOrEmpty(selectedRow[field.Index])) || isRadioButtonList && inserting && !field.AllowNulls),
                            ReadOnly: field.ReadOnly && !(field.IsPrimaryKey && inserting) || readOnly
                        });
                }
            }
            for (i = 0; i < this._externalFilter.length; i++) {
                var filterItem = this._externalFilter[i];
                for (j = 0; j < values.length; j++) {
                    var v = values[j];

                    if (v.Name.toLowerCase() == filterItem.Name.toLowerCase() && v.NewValue == null) {
                        v.NewValue = typeof filterItem.Value == 'string' ? this.convertStringToFieldValue(this.findField(v.Name), filterItem.Value) : filterItem.Value;
                        v.Modified = true;
                        break;
                    }
                }
            }
            return values;
        },
        _enumerateExpressions: function (type, scope, target) {
            var l = [];
            if (this._expressions) {
                for (var i = 0; i < this._expressions.length; i++) {
                    var e = this._expressions[i];
                    if (e.Scope == scope && (type == Web.DynamicExpressionType.Any || e.Type == Web.DynamicExpressionType.RegularExpression) && e.Target == target)
                        Array.add(l, e);
                }
            }
            return l;
        },
        _prepareJavaScriptExpression: function (expression) {
            if (!expression._variables) {
                var vars = [],
                    re = /(\[(\w+)\])|(\$row\.(\w+))/gm,
                    m = re.exec(expression.Test);
                while (m) {
                    var fieldName = m[2] || m[4],
                        found = false;
                    for (var i = 0; i < vars.length; i++) {
                        if (vars[i].name == fieldName) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        var f = this.findField(fieldName);
                        if (f)
                            Array.add(vars, { 'name': fieldName, 'regex': new RegExp('(\\[' + fieldName + '\\])|(\\$row\\.' + fieldName + ')', 'g'), 'replace': String.format('this._javaScriptRowValue({0})', f.Index) });
                    }
                    m = re.exec(expression.Test);
                }
                expression._variables = vars;
                expression.Test = expression.Test.replace(/\[(\w+)\.(\w+)\]/gi, 'this.fieldValue(\'$2\',\'$1\')');
            }
        },
        _javaScriptRowValue: function (fieldIndex, source) {
            var that = this,
                v = that._javaScriptRow[fieldIndex],
                field, dataViewId, extension, dataView, dataViewRow, selectedKey,
                fv, result;
            if (_touch) {
                field = that._allFields[fieldIndex];
                if (field.Type == 'DataView') {
                    result = that._javaScriptValues[field.Name];
                    if (!result) {
                        dataViewId = field._dataViewId;
                        if (dataViewId) {
                            result = { _ready: true, _selected: false };
                            dataView = findDataView(field._dataViewId);
                            selectedKey = dataView._selectedKey;
                            if (selectedKey.length) {
                                extension = dataView.extension();
                                if (extension) {
                                    dataViewRow = extension.commandRow();
                                    if (dataViewRow && dataViewRow.length) {
                                        dataView._allFields.forEach(function (f) {
                                            result[f.Name] = dataViewRow[f.Index];
                                        });
                                        result._selected = selectedKey[0] !== undefined;
                                    }
                                }
                            }
                        }
                        else
                            result = { _ready: false, _selected: false };
                        that._javaScriptValues[field.Name] = result;
                    }
                }
                else
                    result = v;
                return result;
            }
            else {
                if (!that._javaScriptRowConvert)
                    return v;
                field = that._allFields[fieldIndex];
                fv = { Name: field.Name, NewValue: v, Modified: true };
                return that._validateFieldValues([fv], false, false, false) ? fv.NewValue : null;
            }
        },
        _evaluateJavaScriptExpressions: function (expressions, row, concatenateResult) {
            var that = this,
                result = concatenateResult ? '' : null,
                i, j, v, r,
                exp, script;
            for (i = 0; i < expressions.length; i++) {
                exp = expressions[i];
                if (exp.Type == Web.DynamicExpressionType.ClientScript) {
                    that._prepareJavaScriptExpression(exp);
                    script = exp._script;
                    if (!script) {
                        script = exp.Test;
                        for (j = 0; j < exp._variables.length; j++) {
                            v = exp._variables[j];
                            script = script.replace(v.regex, v.replace);
                        }
                        exp._script = eval('(function(){return ' + script + '})');
                        if (exp._script == null)
                            exp._script = script;
                        else
                            script = exp._script;
                    }
                    if (script) {
                        that._javaScriptValues = {};
                        that._javaScriptRow = row;
                        that._javaScriptRowConvert = that.editing();
                        //                        try {
                        r = typeof script == 'string' ? eval(script) : script.call(this);
                        if (concatenateResult) {
                            if (r) {
                                if (result == null) result = exp.Result;
                                else result += ' ' + exp.Result;
                            }
                        }
                        else
                            return exp.Result == null ? r : exp.Result;
                        //}
                        //finally {
                        //    that._javaScriptRow = null;
                        //}
                    }
                }
            }
            return result;
        },
        _validateFieldValueFormat: function (field, v, skipDateAdjustment) {
            var error = null;
            switch (field.Type) {
                case 'SByte':
                case 'Byte':
                case 'Int16':
                case 'Int32':
                case 'UInt32':
                case 'Int64':
                case 'Single':
                case 'Double':
                case 'Decimal':
                case 'Currency':
                    var newValue = v.NewValue;
                    if (typeof (newValue) != 'number')
                        newValue = Number.tryParse(newValue);
                    if (/*String.format('{0}', newValue)*/isNaN(newValue) || newValue == null)
                        error = resourcesValidator.NumberIsExpected;
                    else {
                        if (newValue != null && field.Type.match(/int|byte/i))
                            newValue = Math.round(newValue);
                        v.NewValue = newValue;
                    }
                    break;
                case 'Boolean':
                    try {
                        v.NewValue = String.isInstanceOfType(v.NewValue) ? Boolean.parse(v.NewValue) : v.NewValue;
                    }
                    catch (e) {
                        error = resourcesValidator.BooleanIsExpected;
                    }
                    break;
                case 'Date':
                case 'DateTime':
                    //case 'DateTimeOffset':
                    if (typeof v.NewValue == 'string') {
                        var dataFormatString = field.DataFormatString;
                        newValue = !isNullOrEmpty(dataFormatString) ? Date.parseLocale(v.NewValue, dataFormatString.match(/\{0:([\s\S]*?)\}/)[1]) : Date.parse(v.NewValue);
                        if (!newValue && dataFormatString) {
                            newValue = Date.parse(v.NewValue);
                            newValue = isNaN(newValue) ? null : new Date(newValue);
                        }
                        if (!newValue && field.DateFmtStr && field.TimeFmtStr)
                            newValue = Date.tryParseFuzzyDateTime(v.NewValue, dataFormatString);
                        if (!newValue)
                            newValue = Date.tryParseFuzzyDate(v.NewValue, dataFormatString);
                        if (!newValue && field.TimeFmtStr)
                            newValue = Date.tryParseFuzzyTime(v.NewValue);
                        if (!newValue) {
                            error = resourcesValidator.DateIsExpected;
                        }
                        else
                            v.NewValue = newValue;
                    }
                    else if (isNaN(v.NewValue.getTime()))
                        error = resourcesValidator.DateIsExpected;
                    break;
            }
            return error;
        },
        validate: function (values) {
            for (var i = 0; i < values.length; i++)
                if (values[i].Error)
                    return false;
            return true;
        },
        _validateFieldValues: function (values, displayErrors, focusedCell, skipDateAdjustment) {
            var that = this,
                valid = true,
                sb = new Sys.StringBuilder(),
                i,
                newValue, oldValue,
                v, field, error;
            for (i = 0; i < values.length; i++) {
                v = values[i];
                newValue = v.NewValue;
                oldValue = v.OldValue;
                field = this.findField(v.Name);
                if (!field || focusedCell && field.ColIndex != focusedCell.colIndex) continue;
                error = null;
                if (field.ReadOnly && field.IsPrimaryKey) {
                    if (newValue == null && oldValue != null)
                        v.NewValue = oldValue;
                }
                else if (v.Modified /*&& (typeof (__designerMode) == 'undefined' || !v.ReadOnly && !(field.ReadOnly && field.IsPrimaryKey))*/) {
                    // see if the field is blank
                    if (!field.AllowNulls && (!field.HasDefaultValue || resourcesValidator.EnforceRequiredFieldsWithDefaultValue)) {
                        if (isBlank(newValue) && !field.Hidden && !field.isReadOnly())
                            error = resourcesValidator.RequiredField;
                    }
                    // convert blank values to "null"
                    if (!error && isBlank(newValue))
                        v.NewValue = null;
                    // convert to the "typed" value
                    if (!error && v.NewValue != null && !field.IsMirror && (!field.Hidden || v.Modified)) {
                        var fieldError = this._validateFieldValueFormat(field, v, skipDateAdjustment);
                        if (!field.isReadOnly())
                            error = fieldError;
                    }
                    if (!error) {
                        var expressions = this._enumerateExpressions(Web.DynamicExpressionType.RegularExpression, Web.DynamicExpressionScope.Field, v.Name);
                        for (var j = 0; j < expressions.length; j++) {
                            var exp = expressions[j];
                            var s = v.NewValue ? v.NewValue : '';
                            try {
                                var re = new RegExp(exp.Test);
                                var m = re.exec(s);
                                if (exp.Result.match(/\$(\d|\'\`)/)) {
                                    if (m) v.NewValue = s.replace(re, exp.Result);
                                }
                                else {
                                    if (!m) error = error ? error += exp.Result : exp.Result;
                                }
                            }
                            catch (ex) {
                            }
                        }
                    }
                    // see if the value has been modified
                    v.Modified = field.Type.startsWith('DateTime') ? ((v.NewValue == null ? null : v.NewValue.toString()) != (v.OldValue == null ? null : v.OldValue.toString())) : v.NewValue != v.OldValue;
                    //if (field.Type.startsWith('DateTime')) {
                    //    v.OldValue = this.convertFieldValueToString(field, v.OldValue);
                    //    v.NewValue = this.convertFieldValueToString(field, v.NewValue);
                    //}
                }
                v.Error = error;
                // display/hide the error as needed
                if (_touch) {
                    if (error && displayErrors && valid) {
                        this._focus(field.Name, error);
                        valid = false;
                    }
                }
                else {
                    var errorElement = $get(this.get_id() + '_Item' + field.Index + '_Error');
                    if (errorElement && displayErrors) {
                        //Sys.UI.DomElement.setVisible(errorElement, error != null);
                        Sys.UI.DomElement.setVisible(errorElement, false);
                        errorElement.innerHTML = error;
                        if (error != null && valid)
                            this._showFieldError(field, error);
                        var elem = errorElement.parentNode;
                        while (elem && elem.tagName != 'BODY') {
                            if (!$common.getVisible(elem)) {
                                errorElement = null;
                                break;
                            }
                            elem = elem.parentNode;
                        }
                    }
                    if (error && displayErrors) {
                        if (valid) {
                            var lastFocusedCell = this._lastFocusedCell;
                            if (lastFocusedCell) {
                                this._focusCell(-1, -1, false);
                                this._focusCell(lastFocusedCell.rowIndex, lastFocusedCell.colIndex, true);
                                this._lastFocusedCell = null;
                            }
                            this._focus(field.Name);

                        }
                        valid = false;
                        if (!errorElement) sb.append(_app.formatMessage('Attention', field.Label + ": " + error));
                    }
                }
            }
            if (valid && _app.upload())
                $(this._allFields).each(function () {
                    var f = this,
                        test;
                    if (f.OnDemand) {
                        test = _app.upload('validate', { container: findBlobContainer(that, f), dataViewId: that._id, fieldName: f.Name });
                        if (!f.AllowNulls && !test && displayErrors) {
                            valid = false;
                            error = resourcesValidator.Required;
                            if (_touch)
                                that._focus(f.Name, error);
                            else
                                sb.appendFormat('<b>{0}</b>: {1}', f.Label, error);
                            return false;
                        }
                    }
                });
            if (!displayErrors) valid = true;
            if (!valid)
                if (this._skipInvoke)
                    this._validationError = sb.toString();
                else
                    _app.showMessage(sb.toString());
            sb.clear();
            return valid;
        },
        _fieldIsInExternalFilter: function (field) {
            return this._findExternalFilterItem(field) != null;
        },
        _findExternalFilterItem: function (field) {
            for (var i = 0; i < this._externalFilter.length; i++) {
                var filterItem = this._externalFilter[i];
                if (field.Name.toLowerCase() == filterItem.Name.toLowerCase())
                    return filterItem;
            }
            return null;
        },
        _internalFocus: function () {
            var elem = $get(this._focusId);
            if (elem)
                try {
                    elem.value = '';
                    elem.value = this._focusText;
                    Sys.UI.DomElement.setCaretPosition(elem, this._focusText.length);
                }
                catch (err) {
                }
        },
        _showFieldError: function (field, message) {
            var error = this._get('_Item', field.Index + '_Error');
            if (error) {
                Sys.UI.DomElement.setVisible(error, message != null);
                if (message) {
                    this._skipErrorReset = true;
                    error.style.marginLeft = '0px';
                    error.style.marginTop = '0px';
                    error.innerHTML = String.format('{0} <a href="javascript:" onclick="Sys.UI.DomElement.setVisible(this.parentNode, false);$find(\'{2}\')._focus(\'{3}\');return false" class="Close" title="{1}"><span>&nbsp;</span></a>', message, resourcesModalPopup.Close, this.get_id(), field.Name);
                    if (this.get_isForm()) {
                        var pp = $common.getPaddingBox(error.previousSibling ? error.previousSibling : error);
                        error.style.marginLeft = pp.left + 'px';
                        error.style.marginTop = 1 + 'px';
                    }
                    else {
                        var scrolling = _app.scrolling(); // $common.getScrolling();
                        var cb = $common.getClientBounds();
                        var eb = $common.getBounds(error);
                        var deltaX = eb.x + eb.width - (scrolling.x + cb.width);
                        if (deltaX < 0)
                            deltaX = 0;
                        var pb = $common.getBounds(error.parentNode);
                        pp = $common.getPaddingBox(error.parentNode);
                        var nextSibling = error.nextSibling;
                        while (nextSibling && nextSibling.nodeType != 1)
                            nextSibling = nextSibling.nextSibling;
                        var b = $common.getBounds(nextSibling ? nextSibling : error);
                        error.style.marginLeft = (-(b.x - pb.x + 1 + deltaX)) + 'px';
                        error.style.marginTop = (b.height + pp.bottom) + 'px'; //(pb.height - (b.y - pb.y)) + 'px';
                    }
                }
            }
        },
        _focus: function (fieldName, message) {
            if (this._skipFocus) return;
            if (_touch) {
                this.extension().focus(fieldName, message);
                return;
            }
            if (message) {
                for (var i = 0; i < this.get_fields().length; i++) {
                    this._showFieldError(this.get_fields()[i]);
                }
            }
            var cell = this._get_focusedCell();
            if (cell && this.editing() && this._id == _app._activeDataSheetId) {
                if (!isNullOrEmpty(fieldName)) {
                    var field = null;
                    var cellChanged = false;
                    for (i = 0; i < this._fields.length; i++) {
                        field = this._fields[i];
                        if (field.Name == fieldName) {
                            this._focusCell(cell.rowIndex, cell.colIndex, false);
                            cellChanged = cell.colIndex != i;
                            cell = { rowIndex: this._selectedRowIndex, colIndex: i };
                            if (!this._continueAfterScript)
                                this._saveAndNew = false;
                            break;
                        }
                    }
                    if (!isNullOrEmpty(message) && field) {
                        if (cellChanged) {
                            this._focusCell(cell.rowIndex, cell.colIndex, true);
                            this.refresh(true);
                        }
                        this._showFieldError(field, message);
                    }
                }
                var cellElem = this._focusCell(cell.rowIndex, cell.colIndex, true);
                if (cellElem) {
                    var list = cellElem.getElementsByTagName('input');
                    var canFocus = false;
                    for (i = 0; i < list.length; i++)
                        if (list[i].type != 'hidden') {
                            canFocus = true;
                            break;
                        }
                    if (!canFocus)
                        list = cellElem.getElementsByTagName('textarea');
                    if (list.length == 0)
                        list = cellElem.getElementsByTagName('select');
                    if (list.length == 0)
                        list = cellElem.getElementsByTagName('a');
                    for (i = 0; i < list.length; i++) {
                        var elem = list[i];
                        if (elem.tagName != 'INPUT' || elem.type != 'hidden') {
                            if ((elem.tagName == 'INPUT' || elem.tagName == 'TEXTAREA') && this._pendingChars != null) {
                                this._focusText = this._pendingChars;
                                this._focusId = elem.id;
                                var self = this;
                                setTimeout(function () {
                                    self._internalFocus();
                                }, 50);
                            }
                            else
                                Sys.UI.DomElement.setFocus(elem);
                            break;
                        }
                    }
                }
                this._pendingChars = null;
                return;
            }
            if (isNullOrEmpty(fieldName) && !isNullOrEmpty(this._focusedFieldName)) {
                field = this.findField(this._focusedFieldName);
                if (field) fieldName = field.Name;
            }
            this._focusedFieldName = fieldName;
            for (i = 0; i < this.get_fields().length; i++) {
                field = this.get_fields()[i];
                var autoComplete = field.ItemsStyle == 'AutoComplete' && (field.Name == fieldName || field.AliasName == fieldName || isNullOrEmpty(fieldName));
                if (!field.ReadOnly && (fieldName == null || field.Name == fieldName || autoComplete)) {
                    var elemId = this.get_id() + '_Item' + (autoComplete ? field.AliasIndex : field.Index);
                    switch (field.ItemsStyle) {
                        case 'RadioButtonList':
                        case 'CheckBoxList':
                            elemId += '_0';
                            break;
                        case 'Lookup':
                            elemId += '_ShowLookupLink';
                            break;
                    }
                    var element = $get(elemId),
                        elementFocus = $(element).data('focus');
                    var c = $get(String.format('{0}_ItemContainer{1}', this.get_id(), field.Index));
                    var cat = this._categories[field.CategoryIndex];
                    var categoryTabIndex = Array.indexOf(this._tabs, cat.Tab);
                    if (fieldName && categoryTabIndex >= 0) this.set_categoryTabIndex(categoryTabIndex);
                    this._toggleCategoryVisibility(field.CategoryIndex, true);
                    if (element && (!c || Sys.UI.DomElement.getVisible(c) || elementFocus)) {
                        if (fieldName || (categoryTabIndex == this.get_categoryTabIndex() || this._tabs.length == 0)) {
                            if (categoryTabIndex >= 0) this.set_categoryTabIndex(categoryTabIndex);
                            try {
                                if (message) {
                                    this._showFieldError(field, message);
                                }
                                if (elementFocus)
                                    elementFocus();
                                else
                                    Sys.UI.DomElement.setFocus(element);
                            }
                            catch (ex) {

                            }
                            break;
                        }
                    }
                }
            }
        },
        _serverFocus: function (fieldName, message) {
            this._focus(fieldName, message);
            if (this.get_isDataSheet()) {
                this.refresh(true);
                this._focus(fieldName, message);
            }
        },
        _saveTabIndexes: function () {
            this._lastSavedTabIndexes = this._savedTabIndexes;
            this._savedTabIndexes = [];
            for (var i = 0; i < _app._tagsWithIndexes.length; i++) {
                var tags = document.getElementsByTagName(_app._tagsWithIndexes[i]);
                for (var j = 0; j < tags.length; j++) {
                    var elem = tags[j];
                    if (elem)
                        Array.add(this._savedTabIndexes, { element: elem, tabIndex: elem.tabIndex });
                }
            }
        },
        _restoreTabIndexes: function () {
            if (this._savedTabIndexes) {
                for (var i = 0; i < this._savedTabIndexes.length; i++) {
                    this._savedTabIndexes[i].element.tabIndex = this._savedTabIndexes[i].tabIndex;
                    delete this._savedTabIndexes[i].element;
                }
                Array.clear(this._savedTabIndexes);
            }
            this._savedTabIndexes = this._lastSavedTabIndexes;
            this._lastSavedTabIndexes = null;
        },
        _selectKeyByRowIndex: function (rowIndex) {
            this._selectKeyByRow(this._rows[rowIndex]);
        },
        _selectKeyByRow: function (row) {
            if (!row) return;
            this._selectedRow = row;
            var oldKey = this._selectedKey;
            this._selectedKey = [];
            this._selectedKeyFilter = []
            for (var i = 0; i < this._keyFields.length; i++) {
                var field = this._keyFields[i];
                Array.add(this._selectedKey, row[field.Index]);
                Array.add(this._selectedKeyFilter, field.Name + ':=' + this.convertFieldValueToString(field, row[field.Index]));
                if (oldKey && (oldKey.length < i || (oldKey[i] != this._selectedKey[i]))) oldKey = null;
            }
            this.updateSummary();
            if (!oldKey && !this._raiseSelectedDelayed) this.raiseSelected();
        },
        _showWait: function (force) {
            if (_touch) return;
            //   $.mobile.loading('show');
            var waitHtml = resources.Common.WaitHtml;
            if (this.get_fields() == null || force) {
                this._container.innerHTML = this.get_isModal() || this.get_lookupField() ? waitHtml : '<div class="dataview-busy-whitespace"></div>';
                $(this._container).find('.Wait').addClass('dataview');
            }
            else {
                var wait = $get(this.get_id() + '_Wait');
                if (wait) {
                    this._oldWaitHTML = wait.innerHTML;
                    wait.innerHTML = waitHtml;
                }
            }
            var extension = this.extension();
            if (extension)
                extension.wait();
        },
        _hideWait: function () {
            if (this._oldWaitHTML != null) {
                var waitElement = $get(this.get_id() + '_Wait');
                if (waitElement) waitElement.innerHTML = this._oldWaitHTML;
            }
        },
        _get_colSpan: function () {
            return this.get_isForm() ? 2 : this.get_fields().length +
                (this.multiSelect() ? 1 : 0) +
                (this.get_showIcons() ? 1 : 0) +
                (this.get_isDataSheet() ? 1 : 0) +
                (this._actionColumn ? 1 : 0);
        },
        _renderCreateNewBegin: function (sb, field) { if (!isNullOrEmpty(field.ItemsNewDataView)) sb.append('<table cellpadding="0" cellspacing="0"><tr><td>'); },
        _renderCreateNewEnd: function (sb, field) {
            if (!isNullOrEmpty(field.ItemsNewDataView)) {
                sb.append('</td><td>');
                if (this.get_enabled())
                    sb.appendFormat('<a href="#" class="CreateNew" onclick="$find(\'{0}\').createNewLookupValue({1});return false" id="{0}_Item{1}_CreateNewLookupLink" title="{2}"{3}>&nbsp;</a>',
                        this.get_id(), field.Index, String.format(resources.Lookup.NewToolTip, field.Label), String.format(' tabindex="{0}"', $nextTabIndex()));
                sb.append('</td></tr></table>');
            }
        },
        _raisePopulateDynamicLookups: function () {
            var that = this;
            if (that._hasDynamicLookups && that.editing() && that._skipPopulateDynamicLookups != true)
                if (that._survey)
                    _app.survey('populateItems', {
                        dataView: that, callback: function (populatedList) {
                            var result = { Values: [] };
                            $(that._allFields).each(function () {
                                var f = this,
                                    allow;
                                $(populatedList).each(function () {
                                    if (this.Name == f.Name) {
                                        allow = true;
                                        return false;
                                    }
                                });
                                if (f.ItemsAreDynamic && allow)
                                    result.Values.push({ Name: f.Name, NewValue: f.Items });
                            });
                            that._populateDynamicLookups(result);
                        }
                    });
                else
                    that.executeCommand({ 'commandName': 'PopulateDynamicLookups', 'commandArgument': '', 'causesValidation': false });
            that._skipPopulateDynamicLookups = false;
        },
        _raiseCalculate: function (field, triggerField) {
            this.executeCommand({ 'commandName': 'Calculate', 'commandArgument': field.Name, 'causesValidation': false, 'trigger': triggerField.Name });
        },
        get_currentRow: function () {
            //return this.inserting() ? (this._newRow ? this._newRow : []) : this.get_selectedRow();
            var that = this;
            return that.inserting() ?
                (_touch ? that.editRow() : (that._newRow ? that._newRow : [])) :
                that.get_selectedRow();
        },
        fieldValue: function (fieldName, source) {
            if (fieldName == '_wizard') {
                var layout = $('#' + this._id + ' [data-layout]'),
                    config = layout.data('wizard-config');
                return config ? config.active : 0;
            }
            var dataView = !source || source.length == 0 ? this : (source.match(/^master$/i) ? this.get_master() : $find(source));
            if (!dataView) return null;
            if (!dataView._allFields)
                return null;
            var f = dataView.findField(fieldName);
            if (!f) return null;
            var r = dataView._clonedRow;
            if (!r)
                r = dataView._cloneChangedRow();
            return r ? r[f.Index] : null;
        },
        _useLEVs: function (row) {
            if (row && this._allowLEVs) {
                var r = this._get_LEVs();
                if (r.length > 0) {
                    for (i = 0; i < r[0].length; i++) {
                        var v = r[0][i];
                        f = this.findField(v.Name);
                        if (f && f.AllowLEV) {
                            if (this._lastCommandName == 'New' && v.Modified && v.NewValue != null) {
                                var copy = true;
                                for (var k = 0; k < this._externalFilter.length; k++) {
                                    if (this._externalFilter[k].Name.toLowerCase() == v.Name.toLowerCase()) {
                                        copy = false;
                                        break;
                                    }
                                }
                                if (copy)
                                    row[f.Index] = v.NewValue;
                            }
                            f._LEV = v.Modified ? v.NewValue : null;
                        }
                    }
                }
            }
        },
        _configure: function (row) {
            if (!this._requiresConfiguration) return;
            if (!row) row = this.get_currentRow();
            if (!row) return;
            for (var i = 0; i < this._allFields.length; i++) {
                var f = this._allFields[i];
                if (!isNullOrEmpty(f.Configuration)) {
                    var iterator = /\s*(\w+)=(\w+)\s*?($|\n)/g;
                    var m = iterator.exec(f.Configuration);
                    while (m) {
                        var sourceField = this.findField(m[2]);
                        if (sourceField) {
                            var v = row[sourceField.Index];
                            if (v) f[m[1]] = v;
                        }
                        m = iterator.exec(f.Configuration);
                    }
                }
            }
        },
        _focusQuickFind: function (force) {
            if (!this._quickFindFocused || force == true) {
                this._lostFocus = true;
                try {
                    Sys.UI.DomElement.setFocus(this.get_quickFindElement());
                    //this.get_quickFindElement().select();
                    //this.get_quickFindElement().focus();
                }
                catch (ex) {
                }
                this._quickFindFocused = true;
            }
        },
        _restoreEmbeddedViews: function () {
            if (!this._embeddedViews) return;
            for (var i = 0; i < this._embeddedViews.length; i++) {
                var ev = this._embeddedViews[i];
                ev.parent.appendChild(ev.view._element);
                delete ev.parent;
                ev.view = null;
            }
            Array.clear(this._embeddedViews);
        },
        _incorporateEmbeddedViews: function () {
            if (!this._embeddedViews) return;
            for (var i = 0; i < this._embeddedViews.length; i++) {
                var ev = this._embeddedViews[i];
                var placeholder = $get('v_' + ev.view.get_id());
                var elem = ev.view._element;
                ev.parent = elem.parentNode;
                placeholder.appendChild(elem);
            }
        },
        raiseSelectedDelayed: function () {
            if (this._raiseSelectedDelayed) {
                this._raiseSelectedDelayed = false;
                this.raiseSelected();
                this._forceChanged = false;
            }
        },
        refreshChildren: function () {
            var dataView = this.get_parentDataView(this);
            dataView._forceChanged = true;
            dataView._raiseSelectedDelayed = true;
            if (!dataView._isBusy)
                dataView.refresh(true);
        },
        _render: function (refreshExtension) {
            if (_touch) {
                if (refreshExtension)
                    this._refreshExtension();
                this.get_parentDataView(this).raiseSelectedDelayed();
            }
            else {
                this._restoreEmbeddedViews();
                this._detachBehaviors();
                this._disposeSearchBarExtenders();
                //var checkWidth = this.get_view() && this.get_isForm()/* this.get_view().Type == 'Form'*/ && this._numberOfColumns > 1;
                //var width = this.get_element().offsetWidth;
                this._useLEVs();
                this._configure();
                this._internalRender();
                if (this._viewMessages)
                    this.showViewMessage(this._viewMessages[this.get_viewId()]);
                //if (!this.get_isModal() && checkWidth && width < this.get_element().offsetWidth) {
                //            this._dittoCollectedValues();
                //            var oldNumberOrColumns = this._numberOfColumns;
                //            this._numberOfColumns = 1;
                //            this._ignoreColumnIndex = true;
                //            this._internalRender();
                //            this._numberOfColumns = oldNumberOrColumns;
                //            this._ignoreColumnIndex = false;
                //}
                this._raisePopulateDynamicLookups();
                if (!this.get_modalAnchor() && this.get_lookupField())
                    this._focusQuickFind();
                this.raiseSelectedDelayed();
                if (this._scrollIntoView) {
                    this._scrollIntoView = false;
                    var bounds = $common.getBounds(this._element);
                    var scrolling = _app.scrolling(); // $common.getScrolling();
                    if (bounds.y < scrolling.y)
                        this._element.scrollIntoView(true);
                }
                this._incorporateEmbeddedViews();
                if (this.get_isModal())
                    this._adjustModalPopupSize();
                if (this.get_searchOnStart() && this.get_isGrid() /*this.get_viewType() == 'Grid'*/) {
                    this._focusSearchBar();
                    this.set_searchOnStart(false);
                }
                if (this.get_isDataSheet()) {
                    var fc = this._get_focusedCell();
                    if (this.inserting() && !fc) {
                        this._startInputListenerOnCell(0, 0);
                    }
                    else if (fc && this._id == _app._activeDataSheetId) {
                        this._skipCellFocus = true;
                        this._focusCell(-1, -1, true);
                    }
                }
                if (this.get_isGrid() && this._synced) {
                    this._synced = false;
                    if (this._selectedRowIndex == null) {
                        this._forgetSelectedRow(true);
                        this.refresh(true);
                    }
                }
                this._syncKeyFilter();
            }
        },
        _syncKeyFilter: function () {
            var key = this.get_selectedKey();
            if (key.length > 0 && this._selectedKeyFilter.length == 0) {
                for (var i = 0; i < this._keyFields.length; i++) {
                    var f = this._keyFields[i];
                    Array.add(this._selectedKeyFilter, f.Name + ':=' + this.convertFieldValueToString(f, key[i]));
                }
            }
        },
        _mergeRowUpdates: function (row) {
            if (this._lastCommandName == 'BatchEdit') {
                var batchEdit = this._batchEdit = [];
                var allFields = this._allFields;
                $(this._element).find('.BatchSelect input:checkbox:checked').each(function () {
                    var m = this.id.match(/BatchSelect(\d+)$/);
                    if (m) {
                        var f = allFields[parseInteger(m[1])];
                        Array.add(batchEdit, f.Name);
                    }
                });
            }
            this._originalRow = null;
            this._useLEVs(row);
            if (this.editing() && this._ditto) {
                this._originalRow = Array.clone(row);
                for (var i = 0; i < this._ditto.length; i++) {
                    var d = this._ditto[i];
                    var f = this.findField(d.name);
                    if (f && !(f.ReadOnly && f.IsPrimaryKey))
                        row[f.Index] = d.value;
                }
                delete this._ditto;
            }
            this._configure(row);
            this._mergedRow = row;
        },
        _removeRowUpdates: function () {
            var row = this._mergedRow;
            if (!row) return;
            if (this._originalRow) {
                for (var i = 0; i < this._originalRow.length; i++)
                    row[i] = this._originalRow[i];
            }
            this._mergedRow = null;
        },
        _internalRender: function () {
            this._multipleSelection = null;
            this._dynamicActionButtons = false;
            var viewType = this.get_viewType(),
                sb = new Sys.StringBuilder(),
                isForm;
            if (this.get_mode() == Web.DataViewMode.Lookup) {
                var field = this._fields[0];
                var v = this.get_lookupText();
                if (v == null) v = resources.Lookup.SelectLink;
                var s = field.format(v);
                this._renderCreateNewBegin(sb, field);
                sb.appendFormat('<table cellpadding="0" cellspacing="0" class="DataViewLookup"><tr><td><a href="javascript:" onclick="$find(\'{0}\').showLookup({1});return false" class="Select" id="{0}_Item{1}_ShowLookupLink" title="{3}" tabindex="{7}"{8}>{2}</a><a href="#" class="Clear" onclick="$find(\'{0}\').clearLookupValue({1});return false" id="{0}_Item{1}_ClearLookupLink" title="{5}" tabindex="{7}">&nbsp;</a></td></tr></table><input type="hidden" id="{0}_Item{1}" value="{4}"/><input type="hidden" id="{0}_Text{1}" value="{6}"/>',
                    this.get_id(), field.Index, this.htmlEncode(field, s), String.format(resourcesLookup.SelectToolTip, field.Label), this.get_lookupValue(), String.format(resources.Lookup.ClearToolTip, field.Label), _app.htmlAttributeEncode(s), $nextTabIndex(), this.get_enabled() ? '' : ' disabled="true" class="Disabled"');
                this._renderCreateNewEnd(sb, field);
                this.get_element().appendChild(this._container);
                this._container.innerHTML = sb.toString();
                if (this.get_lookupValue() == '' || !this.get_enabled()) $get(this.get_id() + '_Item0_ClearLookupLink').style.display = 'none';
            }
            else {
                isForm = viewType == 'Form';
                sb.appendFormat('<table class="DataView {1}_{2}{3}{4} {5}Type" cellpadding="0" cellspacing="0"{0}>', this.get_isModal() ? String.format(' style="width:{0}px"', this._container.offsetWidth - 20) : '', this.get_controller(), this.get_viewId(), this._numberOfColumns > 0 ? ' MultiColumn' : '', this._tabs.length > 0 ? ' Tabbed' : '', viewType);
                if (isForm)
                    this._renderFormView(sb);
                else
                    this._renderGridView(sb);
                sb.append('</table>');
                if (this._mergedRow) {
                    var cell = this._get_focusedCell();
                    var inserting = this.inserting();
                    var isDataSheet = this.get_isDataSheet();
                    for (var i = 0; i < this._allFields.length; i++) {
                        var f = this._allFields[i];
                        var unfocusedCell = cell && i != this._fields[cell.colIndex].Index;
                        if (f.Hidden && (!f.IsPrimaryKey || inserting) || unfocusedCell || !cell && inserting && isDataSheet) {
                            v = this._mergedRow[i];
                            if (v != null || isDataSheet && !inserting/* || isDataSheet && (!f.IsPrimaryKey || !f.Hidden)*/)
                                sb.appendFormat('<input id="{0}_Item{1}" type="hidden" value="{2}"/>', this.get_id(), i, _app.htmlAttributeEncode(v != null ? f.format(v) : ''));
                        }
                    }
                }
                this._container.innerHTML = sb.toString();
                if (this._multipleSelection != null && this._multipleSelection == true)
                    $get(this.get_id() + '_ToggleButton').checked = true;
                this._attachBehaviors();
                this._updateVisibility();
                if (this.editing()) {
                    if (this._lastCommandName == 'BatchEdit')
                        $(this._element).find('div.BatchSelect input:checkbox:checked').each(function () {
                            _app._updateBatchSelectStatus(this, isForm);
                        });
                    this._focus();
                }
            }
            sb.clear();
            this._updateChart();
            this._updateSearchBar();
            this._removeRowUpdates();
            this._fixWidthOfColumns();
            this._fixHeightOfRow(true);
            this._refreshExtension();
        },
        _refreshExtension: function () {
            var extension = this.extension();
            var lastExtension = this._lastExtension;
            if (extension != lastExtension) {
                if (lastExtension)
                    lastExtension.hide();
                if (extension)
                    extension.show();
                this._lastExtension = extension;
            }
            if (extension)
                extension.refresh();
        },
        _get_headerRowElement: function () {
            var rows = this._container.childNodes[0].childNodes[0].childNodes;
            var i = 0;
            while (i < rows.length) {
                if (Sys.UI.DomElement.containsCssClass(rows[i], 'HeaderRow'))
                    return rows[i];
                i++;
            }
            return null;
        },
        _fixWidthOfColumns: function () {
            if ((this.get_isDataSheet() || this.get_isGrid()) && !this.extension()) {
                var headerRow = this._get_headerRowElement();
                if (headerRow) {
                    if (!this._viewColumnSettings)
                        this._viewColumnSettings = [];
                    var fixedColumns = this._viewColumnSettings[this.get_viewId()];
                    if (!fixedColumns) {
                        fixedColumns = [];
                        $(headerRow).addClass('Fixed');
                        // first pass
                        for (var i = 0; i < headerRow.childNodes.length; i++) {
                            var cell = headerRow.childNodes[i];
                            var b = $common.getBounds(cell);
                            if (b.width == 0) {
                                Sys.UI.DomElement.removeCssClass(headerRow, 'Fixed');
                                return;
                            }
                            var pb = $common.getPaddingBox(cell);
                            var bb = $common.getBorderBox(cell);
                            var fc = { w: b.width - pb.horizontal - bb.horizontal, h: b.height - pb.vertical - bb.vertical };
                            Array.add(fixedColumns, fc);
                            //cell.style.width = fc.w + 'px';
                        }
                        var rowBounds = $common.getBounds(headerRow);
                        for (i = 0; i < fixedColumns.length; i++)
                            fixedColumns[i].h = rowBounds.height;
                        this._viewColumnSettings[this.get_viewId()] = fixedColumns;
                        Sys.UI.DomElement.removeCssClass(headerRow, 'Fixed');
                    }
                    //var hb = $common.getBounds(headerRow);
                    //headerRow.style.height = (fixedColumns[0].h - hb.horizontal) + 'px';
                    for (i = 0; i < headerRow.childNodes.length; i++) {
                        fc = fixedColumns[i];
                        if (fc) {
                            var headerCell = headerRow.childNodes[i];
                            headerCell.style.height = fc.h + 'px';
                            headerCell.style.width = fc.w + 'px';
                        }
                    }
                }
            }
        },
        _fixHeightOfRow: function (apply) {
            if ((this.get_isDataSheet() || this.get_isGrid()) && (!apply || this.editing())) {
                var headerRow = this._get_headerRowElement();
                if (!headerRow) return;
                var fc = this._get_focusedCell();
                var rowIndex = this.get_isGrid() ? this._selectedRowIndex : fc.rowIndex;
                if (rowIndex >= 0) {
                    var tBody = headerRow.parentNode;
                    for (var i = 0; i < tBody.childNodes.length; i++)
                        if (tBody.childNodes[i] == headerRow)
                            break;
                    var rowElem = tBody.childNodes[i + rowIndex + 1];
                    if (rowElem) {
                        if (apply) {
                            if (this._selectedRowHeight)
                                rowElem.style.height = this._selectedRowHeight + 'px';
                            //                        if (this._selectedRowHeight)
                            //                            for (i = 0; i < rowElem.childNodes.length; i++) {
                            //                                rowElem.childNodes[i].style.height = (this._selectedRowHeight - 7) + 'px';
                            //                            }
                        }
                        else {
                            var b = $common.getBounds(rowElem);
                            this._selectedRowHeight = b.height;
                        }
                    }
                }
            }
        },
        _updateChart: function () {
            if (this.get_isChart()) {
                var chart = this._get('$Chart');
                var w = chart.offsetWidth;
                if (w < 100)
                    w = chart.parentNode.offsetWidth;
                var pageRequest = this._createParams();
                //delete pageRequest.Transaction;
                delete pageRequest.LookupContextFieldName;
                delete pageRequest.LookupContextController;
                delete pageRequest.LookupContextView;
                delete pageRequest.LookupContext;
                delete pageRequest.LastCommandName;
                delete pageRequest.LastCommandArgument;
                delete pageRequest.Inserting;
                delete pageRequest.DoesNotRequireData;
                var r = _serializer.serialize(pageRequest);
                var chartBounds = $common.getBounds(chart);
                //if (chartBounds.height > 0 && !isNullOrEmpty(chart.src))
                //    chart.style.height = chartBounds.height + 'px';
                var that = this;
                if (that._chartHeight)
                    chart.style.height = that._chartHeight + 'px';
                //chart.src = String.format('{0}ChartHost.aspx?c={1}_{2}&w={3}&r={4}', this.get_baseUrl(), this.get_controller(), this.get_viewId(), w, encodeURIComponent(r));
                $(chart).one('load', function () {
                    try {
                        //alert($(this).height());
                        that._chartHeight = $(this).height();
                    }
                    catch (ex) {
                    }
                }).attr('src', String.format('{0}ChartHost.aspx?c={1}_{2}&w={3}&r={4}', this.get_baseUrl(), this.get_controller(), this.get_viewId(), w, encodeURIComponent(r)));

            }
        },
        _toggleCategoryVisibility: function (categoryIndex, visible) {
            var categoryFields = $get(String.format('{0}$Category${1}', this.get_id(), categoryIndex));
            if (categoryFields) {
                var cat = this.get_categories()[categoryIndex];
                if (!visible) visible = !Sys.UI.DomElement.getVisible(categoryFields);
                //Sys.UI.DomElement.setVisible(categoryFields, visible);
                $(categoryFields).css('display', visible ? '' : 'none');
                var button = $get(String.format('{0}$CategoryButton${1}', this.get_id(), categoryIndex));
                cat.Collapsed = !visible;
                if (visible) {
                    Sys.UI.DomElement.removeCssClass(button, 'Maximize');
                    button.childNodes[0].title = resources.Form.Minimize;
                }
                else {
                    $(button).addClass('Maximize');
                    button.childNodes[0].title = resources.Form.Maximize;
                }
                _body_performResize();
            }
        },
        _processTemplatedText: function (row, text) {
            if (!text) text = '';
            var iterator = /\{(\w+)\}/g;
            var m = iterator.exec(text);
            if (!m) return text;
            var sb = new Sys.StringBuilder();
            var index = 0;
            while (m) {
                sb.append(text.substring(index, m.index));
                var fieldName = m[1];
                index = m.index + fieldName.length + 2;
                var field = this.findField(fieldName);
                if (field) {
                    sb.append('<span class="FieldPlaceholder">');
                    this._renderItem(sb, field, row, false, false, false, false, false, true);
                    sb.append('</span>');
                }
                else
                    sb.appendFormat('[{0}]', fieldName);
                m = iterator.exec(text);
            }
            var lastIndex = text.length - 1;
            if (index < lastIndex)
                sb.append(text.substring(index, lastIndex));
            return sb.toString().replace(/ (id|for)=\".+?\"/g, '');
        },
        _get_selectedTab: function () {
            return this._tabs.length > 0 ? this._tabs[this.get_categoryTabIndex()] : null;
        },
        _renderFormView: function (sb) {
            var isEditing = this.editing();
            this._renderStatusBar(sb);
            this._renderViewDescription(sb);
            if (resources.Form.ShowActionBar) this._renderActionBar(sb);
            var row = this.get_currentRow();
            this._mergeRowUpdates(row);
            this._updateVisibility(row);
            if (this.inserting() && this._expressions) {
                for (i = 0; i < this._expressions.length; i++) {
                    var exp = this._expressions[i];
                    if (exp.Scope == Web.DynamicExpressionScope.DefaultValues && exp.Type == Web.DynamicExpressionType.ClientScript) {
                        f = this.findField(exp.Target);
                        if (f && row[f.Index] == null) {
                            if (isNullOrEmpty(exp.Test))
                                row[f.Index] = exp.Result;
                            else {
                                var r = eval(exp.Test);
                                if (r)
                                    row[f.Index] = isNullOrEmpty(exp.Result) ? r : exp.Result;
                            }
                        }
                    }
                }
            }
            var fieldCount = 0;
            for (i = 0; i < this._allFields.length; i++)
                if (!this._allFields[i].Hidden) fieldCount++;
            var hasButtonsOnTop = /*fieldCount > Web.DataViewResources.Form.SingleButtonRowFieldLimit && */row != null;
            if (hasButtonsOnTop) this._renderActionButtons(sb, 'Top', 'Form');
            var selectedTab = this._get_selectedTab(); //this._tabs.length > 0 ? this._tabs[this.get_categoryTabIndex()] : null;
            if (this._tabs.length > 0) {
                sb.appendFormat('<tr class="TabsRow"><td colspan="{0}" class="TabsBar{1}">', this._get_colSpan(), !hasButtonsOnTop ? ' WithMargin' : '');
                sb.append('<table cellpadding="0" cellspacing="0" class="Tabs"><tr>');
                for (i = 0; i < this._tabs.length; i++)
                    sb.appendFormat('<td id="{2}_Tab{3}" class="Tab{1}" onmouseover="$hoverTab(this,true)" onmouseout="$hoverTab(this,false)"><span class="Outer"><span class="Inner"><span class="Tab"><a href="javascript:" onclick="$find(&quot;{2}&quot;).set_categoryTabIndex({3});return false;" onfocus="$hoverTab(this,true)" onblur="$hoverTab(this,false)" tabindex="{4}">{0}</a></span></span></span></td>', _app.htmlEncode(this._tabs[i]), i == this.get_categoryTabIndex() ? ' Selected' : '', this.get_id(), i, $nextTabIndex());
                sb.append('</tr></table></td></tr>');
            }
            if (!row) this._renderNoRecordsWhenNeeded(sb);
            else {
                var t = this._get_template();
                if (t) {
                    sb.appendFormat('<tr class="CategoryRow"><td valign="top" class="Fields" colspan="{0}">', this._get_colSpan());
                    this._renderTemplate(t, sb, row, true, false);
                    sb.append('</td></tr>');
                }
                else {
                    var categories = this.get_categories();
                    var fields = this.get_fields();
                    var numCols = this._numberOfColumns;
                    if (numCols > 0) {
                        sb.appendFormat('<tr class="Categories"><td class="Categories" colspan="{0}"><table class="Categories"><tr class="CategoryRow">', this._get_colSpan());
                        for (var k = 0; k < numCols; k++) {
                            if (k > 0)
                                sb.append('<td class="CategorySeparator">&nbsp;</td>');
                            sb.appendFormat('<td class="CategoryColumn" valign="top" style="width:{0}%">', 100 / numCols);
                            for (i = 0; i < categories.length; i++) {
                                var c = categories[i];
                                if (c.ColumnIndex == k || this._ignoreColumnIndex) {
                                    if (this.get_isModal()) c.Collapsed = false;
                                    sb.appendFormat('<div id="{0}_Category{1}" class="Category {3}" style="display:{2}">', this.get_id(), i, !selectedTab || selectedTab == c.Tab ? 'block' : 'none', c.Id);
                                    var description = this._processTemplatedText(row, c.Description);
                                    var descriptionText = this._formatViewText(resources.Views.DefaultCategoryDescriptions[description], true, description);
                                    sb.appendFormat('<table class="Category {8}" cellpadding="0" cellspacing="0"><tr><td class="HeaderText"><span class="Text">{0}</span><a href="javascript:" class="MinMax{6}" onclick="$find(\'{2}\')._toggleCategoryVisibility({3});return false;" id="{2}$CategoryButton${3}" style="{5}"><span title="{4}"></span></a><div style="clear:both;height:1px;margin-top:-1px;"></div></td></tr><tr><td class="Description" id="{2}$CategoryDescription${3}" style="display:{7}">{1}</td></tr></table>',
                                        c.HeaderText, descriptionText,
                                        this.get_id(), i, c.Collapsed ? resources.Form.Maximize : resources.Form.Minimize,
                                        categories.length > 1 && !this.get_isModal() ? '' : 'display:none', c.Collapsed ? ' Maximize' : '', isNullOrEmpty(descriptionText) ? 'none' : 'block', c.Id);
                                    var skip = true;
                                    for (j = 0; j < fields.length; j++) {
                                        var field = fields[j];
                                        if (!field.Hidden && field.CategoryIndex == c.Index) {
                                            skip = false;
                                            break;
                                        }
                                    }
                                    if (!skip) {
                                        sb.appendFormat('<table class="Fields" id="{0}$Category${1}" style="{2}"><tr class="FieldsRow"><td class="Fields" valign="top" width="100%">', this.get_id(), i, c.Collapsed ? 'display:none' : '');
                                        if (!isNullOrEmpty(c.Template))
                                            this._renderTemplate(c.Template, sb, row, true, false);
                                        else {
                                            for (j = 0; j < fields.length; j++) {
                                                field = fields[j];
                                                if (!field.Hidden && field.CategoryIndex == c.Index) {
                                                    var m = field.Name.match(/^(_[A-Za-z_]+)\d/);
                                                    sb.appendFormat('<table cellpadding="0" cellspacing="0" class="FieldWrapper {0}"><tr class="FieldWrapper"><td class="Header" valign="top">', m ? m[1] : field.Name);
                                                    this._renderItem(sb, field, row, true, false, false, true);
                                                    sb.appendFormat('</td><td class="Item{0}" valign="top">', isEditing && !field.isReadOnly() ? '' : ' ReadOnly');
                                                    this._renderItem(sb, field, row, true);
                                                    sb.append('</td></tr></table>');
                                                }
                                            }
                                        }
                                        sb.append('</td></tr></table>');
                                    }
                                    sb.append('</div>');
                                }
                            }
                            sb.append('</td>');
                        }
                        sb.append('</tr></table></td></tr>');
                    }
                    else {
                        for (i = 0; i < categories.length; i++) {
                            c = categories[i];
                            description = this._processTemplatedText(row, c.Description);
                            sb.appendFormat('<tr class="CategoryRow {5}" id="{2}_Category{3}" style="display:{4}"><td valign="top" class="Category {5}"><table class="Category {5}" cellpadding="0" cellspacing="0"><tr><td class="HeaderText">{0}</td></tr><tr><td class="Description" id="{2}$CategoryDescription${3}">{1}</td></tr></table></td><td valign="top" class="Fields {5}">',
                                c.HeaderText, this._formatViewText(resources.Views.DefaultCategoryDescriptions[description], true, description), this.get_id(), i, !selectedTab || selectedTab == c.Tab ? '' : 'none', c.Id);
                            if (!isNullOrEmpty(c.Template))
                                this._renderTemplate(c.Template, sb, row, true, false);
                            else {
                                for (j = 0; j < fields.length; j++) {
                                    field = fields[j];
                                    if (!field.Hidden && field.CategoryIndex == c.Index)
                                        this._renderItem(sb, field, row, true);
                                }
                            }
                            sb.append('</td></tr>');
                        }
                    }
                }
            }
            if (row) this._renderActionButtons(sb, 'Bottom', 'Form');
        },
        _updateTabbedCategoryVisibility: function () {
            if (this._tabs && this._tabs.length > 0) {
                var tab = this._tabs[this.get_categoryTabIndex()];
                for (var i = 0; i < this._tabs.length; i++) {
                    var elem = this._get('_Tab', i); //$get(String.format('{0}_Tab{1}', this.get_id(), i));
                    if (elem) {
                        if (i == this.get_categoryTabIndex())
                            $(elem).addClass('Selected');
                        else
                            $(elem).removeClass('Selected');
                    }
                }
                for (i = 0; i < this._categories.length; i++) {
                    var c = this._categories[i];
                    elem = this._get('_Category', i); // $get(String.format('{0}_Category{1}', this.get_id(), i));
                    if (elem) Sys.UI.DomElement.setVisible(elem, c.Tab == tab);
                }
                this._updateVisibility();
            }
            this._adjustModalHeight();
        },
        _renderItem: function (sb, field, row, isSelected, isInlineForm, isFirstRow, headerOnly, trimLongWords, templateMode) {
            var isForm = this.get_isForm()/* this.get_view().Type == 'Form'*/ || isInlineForm;
            var v = row[field.Index];
            if (v != null) v = v.toString();
            var checkBox = null,
                isCheckBoxList = field.ItemsStyle == 'CheckBoxList',
                isEditing = this.editing(),
                hasAlias = field.Index != field.AliasIndex,
                undefinedLookup = hasAlias && !field.ItemsStyle;
            if (isEditing && field.ItemsStyle == 'CheckBox' && field.Items.length == 2) {
                var fv = field.Items[0][0];
                var tv = field.Items[1][0];
                if (fv == 'true') {
                    fv = 'false';
                    tv = 'true';
                }
                if (v == null) v = field.Items[0][0];
                checkBox = String.format('<input type="checkbox" id="{0}_Item{1}"{2} tabindex="{3}" value="{4}" onclick="this.value=this.checked?\'{6}\':\'{5}\';$find(&quot;{0}&quot;)._valueChanged({1});" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});" title="{7}"/>',
                    this.get_id(), field.Index, tv && (v == 'true' || v == tv) ? ' checked' : '', $nextTabIndex(), v, fv, tv, field.ToolTip);
            }
            var readOnly = field.isReadOnly(); // field.ReadOnly || field.TextMode == 4;
            if (isForm) {
                var errorHtml = String.format('<div class="Error" id="{0}_Item{1}_Error" style="display:none"></div>', this.get_id(), field.Index);
                if (!headerOnly) sb.appendFormat('<div class="Item {2}" id="{0}_ItemContainer{1}">', this.get_id(), field.Index, field.Name);
                //if (this._numberOfColumns == 0) sb.append(errorHtml);
                var headerText = (isEditing && undefinedLookup ? this.findFieldUnderAlias(field) : this._allFields[field.AliasIndex]).HeaderText;
                if (checkBox && headerOnly && !(isEditing && readOnly)) {
                    sb.append('<div class="Header">&nbsp;</div>');
                    return;
                }
                if (headerText.length > 0)
                    if (templateMode && checkBox && this._numberOfColumns > 0)
                        sb.append('<div class="Header">&nbsp;</div>');
                    else
                        sb.appendFormat('<div class="Header {5}">{3}<label for="{0}_Item{1}">{2}{4}</label></div>',
                            this.get_id(), field.Index, headerText,
                            this._numberOfColumns > 0 || templateMode ? '' : checkBox,
                            isEditing && !field.AllowNulls && !checkBox && !readOnly && resources.Form.RequiredFieldMarker ? resources.Form.RequiredFieldMarker : '',
                            field.Name);
                if (headerOnly) return;
                if (checkBox == null || this._numberOfColumns > 0)
                    sb.append('<div class="Value">');
            }
            var needObjectRef = !isEditing && !isNullOrEmpty(field.ItemsDataController) && !isCheckBoxList && !isFirstRow && v && this._disableObjectRef != true && !field.tagged('lookup-details-hidden');
            if (needObjectRef && !isForm) sb.append('<table width="100%" cellpadding="0" cellspacing="0" class="ObjectRef"><tr><td>');
            if (isEditing && isSelected && !readOnly) {
                if (undefinedLookup)
                    field = this.findFieldUnderAlias(field);
                var batchEditField = field;
                if (field._LEV != null)
                    sb.append('<table cellpadding="0" cellspacing="0"><tr><td>');
                if (!isForm && checkBox) sb.append(checkBox);
                var hasItemsStyle = !isNullOrEmpty(field.ItemsStyle);
                var isLookup = field.ItemsStyle == 'Lookup';
                var hasContextFields = !isNullOrEmpty(field.ContextFields);
                var isAutoComplete = field.ItemsStyle == 'AutoComplete';
                var lov = field.DynamicItems ? field.DynamicItems : field.Items,
                    isStaticList = hasItemsStyle && !isLookup && !isAutoComplete;
                if (isStaticList && (lov.length == 0 && !isCheckBoxList) && hasContextFields) {
                    lov = [];
                    if ((field.AllowNulls || v == null) || hasContextFields)
                        Array.add(lov, ['', resourcesData.NullValueInForms]);
                    if (v != null)
                        Array.add(lov, [v, row[field.AliasIndex]]);
                }
                else if (field.DynamicItems && !isCheckBoxList) {
                    var hasValue = false;
                    for (var i = 0; i < lov.length; i++) {
                        if (lov[i][0] == v) {
                            hasValue = true;
                            break;
                        }
                    }
                    //if (!hasValue && !isNullOrEmpty(v)) Array.insert(lov, 0, [v, row[field.AliasIndex]]);
                    if (!hasValue) v = null;
                    if ((field.AllowNulls || !hasValue) && !isNullOrEmpty(lov[0][0]))
                        Array.insert(lov, 0, ['', resourcesData.NullValueInForms]);
                }
                else if (isStaticList && !field.AllowNulls && v == null && !isCheckBoxList && lov.length && lov[0][0] != null) {
                    lov = lov.slice(0);
                    lov.splice(0, 0, ['', resourcesData.NullValueInForms]);
                }
                if (checkBox != null) {
                    if (this._numberOfColumns > 0 || templateMode) {
                        sb.append(checkBox);
                        sb.appendFormat('<label for="{0}_Item{1}" title="{3}">{2}</label>', this.get_id(), field.Index, headerText, field.ToolTip);
                    }
                }
                else
                    if (lov.length > 0 || isCheckBoxList || hasItemsStyle && !isLookup && !isAutoComplete) {
                        if (field.ItemsStyle == 'RadioButtonList') {
                            sb.appendFormat('<table cellpadding="0" cellspacing="0" class="RadioButtonList" title="{0}">', field.ToolTip);
                            var columns = field.Columns == 0 ? 1 : field.Columns;
                            var rows = Math.floor(lov.length / columns) + (lov.length % columns > 0 ? 1 : 0);
                            for (var r = 0; r < rows; r++) {
                                sb.append('<tr>');
                                for (var c = 0; c < columns; c++) {
                                    var itemIndex = c * rows + r; //r * columns + c;
                                    if (itemIndex < lov.length) {
                                        var item = lov[itemIndex];
                                        var itemValue = item[0] == null ? '' : item[0].toString();
                                        if (v == null) v = '';
                                        sb.appendFormat(
                                            '<td class="Button"><input type="radio" id="{0}_Item{1}_{2}" name="{0}_Item{1}" value="{3}"{4} tabindex="{6}" onclick="$find(&quot;{0}&quot;)._valueChanged({1})" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});"/></td><td class="Option"><label for="{0}_Item{1}_{2}">{5}<label></td>',
                                            this.get_id(), field.Index, itemIndex, itemValue, itemValue == v ? " checked" : "", this.htmlEncode(field, item[1]), $nextTabIndex());
                                    }
                                    else
                                        sb.append('<td class="Button">&nbsp;</td><td class="Option"></td>');
                                }
                                sb.append('</tr>');
                            }
                            sb.append('</table>');
                        }
                        else if (isCheckBoxList) {
                            var lov2 = (v ? v.split(',') : []);
                            var waitingForDynamicItems = !isNullOrEmpty(field.ContextFields) && !field.DynamicItems;
                            sb.appendFormat('<input type="hidden" id="{0}_Item{1}" name="{0}_Item{1}" value="{2}"/>', this.get_id(), field.Index, waitingForDynamicItems ? v : '');
                            if (waitingForDynamicItems)
                                sb.append(resourcesHeaderFilter.Loading);
                            else {
                                sb.appendFormat('<table cellpadding="0" cellspacing="0" class="RadioButtonList" title="{0}">', field.ToolTip);
                                columns = field.Columns == 0 ? 1 : field.Columns;
                                rows = Math.floor(lov.length / columns) + (lov.length % columns > 0 ? 1 : 0);
                                for (r = 0; r < rows; r++) {
                                    sb.append('<tr>');
                                    for (c = 0; c < columns; c++) {
                                        itemIndex = c * rows + r; //r * columns + c;
                                        if (itemIndex < lov.length) {
                                            item = lov[itemIndex];
                                            itemValue = item[0] == null ? '' : item[0].toString();
                                            if (v == null) v = '';
                                            sb.appendFormat(
                                                '<td class="Button"><input type="checkbox" id="{0}_Item{1}_{2}" name="{0}_Item{1}" value="{3}"{4} tabindex="{6}"  onclick="$find(&quot;{0}&quot;)._valueChanged({1})" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});"/></td><td class="Option"><label for="{0}_Item{1}_{2}">{5}<label></td>',
                                                this.get_id(), field.Index, itemIndex, itemValue, Array.indexOf(lov2, itemValue) != -1 ? " checked" : "", this.htmlEncode(field, item[1]), $nextTabIndex());
                                        }
                                        else
                                            sb.append('<td class="Button">&nbsp;</td><td class="Option"></td>');
                                    }
                                    sb.append('</tr>');
                                }
                                sb.append('</table>');
                            }
                        }
                        else {
                            sb.appendFormat('<select id="{0}_Item{1}" size="{2}" tabindex="{3}" onchange="$find(&quot;{0}&quot;)._valueChanged({1});" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});" title="{4}">', this.get_id(), field.Index, field.ItemsStyle == 'ListBox' ? (field.Rows == 0 ? 5 : field.Rows) : '1', $nextTabIndex(), field.ToolTip);
                            if (v == null) v = '';
                            v = v.toString();
                            for (i = 0; i < lov.length; i++) {
                                item = lov[i];
                                itemValue = item[0];
                                var itemText = itemValue == null && !field.AllowNulls ? resources.Lookup.SelectLink : item[1];
                                if (itemValue == null) itemValue = '';
                                itemValue = itemValue.toString();
                                sb.appendFormat('<option value="{0}"{1}>{2}</option>', itemValue, itemValue == v ? ' selected' : '', this.htmlEncode(field, itemText));
                            }
                            sb.append('</select>');
                        }
                    }
                    else if (!isNullOrEmpty(field.ItemsDataController) && isLookup) {
                        v = row[field.AliasIndex];
                        if (v == null) v = resources.Lookup.SelectLink;
                        var s = this._allFields[field.AliasIndex].format(v);
                        this._renderCreateNewBegin(sb, field);
                        sb.appendFormat('<table cellpadding="0" cellspacing="0" class="Lookup"><tr><td><a href="#" onclick="$find(\'{0}\').showLookup({1});return false" id="{0}_Item{1}_ShowLookupLink" title="{3}" tabindex="{5}">{2}</a><a href="#" class="Clear" onclick="$find(\'{0}\').clearLookupValue({1});return false" id="{0}_Item{1}_ClearLookupLink" title="{7}" tabindex="{6}" style="display:{8}">&nbsp;</a></td></tr></table><input type="hidden" id="{0}_Item{1}" value="{4}"/><input type="hidden" id="{0}_Item{9}" value="{2}"/>',
                            this.get_id(), field.Index, this.htmlEncode(field, s), !isNullOrEmpty(field.ToolTip) ? field.ToolTip : String.format(resources.Lookup.SelectToolTip, field.Label), row[field.Index], $nextTabIndex(), $nextTabIndex(), String.format(resources.Lookup.ClearToolTip, field.Label), row[field.Index] != null ? 'display' : 'none', field.AliasIndex);
                        this._renderCreateNewEnd(sb, field);
                    }
                    else if (field.OnDemand) this._renderOnDemandItem(sb, field, row, isSelected, isForm);
                    else if (field.Editor) {
                        var editor = field.Editor;
                        sb.appendFormat('<input type="hidden" id="{0}_Item{1}" value="{2}"/>', this.get_id(), field.Index, _app.htmlAttributeEncode(v));
                        sb.appendFormat('<iframe src="{0}?id={1}_Item{2}&control={3}" frameborder="0" scrolling="no" id="{1}_Item{2}$Frame" class="FieldEditor {3}" tabindex="{4}"></iframe>', this.resolveClientUrl(this.get_appRootPath() + 'ControlHost.aspx'), this.get_id(), field.Index, editor, $nextTabIndex());
                    }
                    else if (field.Rows > 1) {
                        sb.appendFormat('<textarea id="{0}_Item{1}" tabindex="{2}" onchange="$find(&quot;{0}&quot;)._valueChanged({1})" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});" title="{3}" style="', this.get_id(), field.Index, $nextTabIndex(), field.ToolTip);
                        if (field.TextMode == 3 && !isNullOrEmpty(v))
                            sb.append('display:none;');
                        if (!isForm)
                            sb.append('display:block;width:100%;"');
                        else
                            sb.appendFormat('" cols="{0}"', field.Columns > 0 ? field.Columns : 50);
                        sb.appendFormat(' rows="{0}"', field.Rows);
                        if (_app.supportsPlaceholder && !isNullOrEmpty(field.Watermark))
                            sb.appendFormat(' placeholder="{0}"', _app.htmlAttributeEncode(field.Watermark));
                        sb.append('>');
                        sb.append(field.HtmlEncode ? this.htmlEncode(field, v) : v);
                        sb.append('</textarea>');
                        if (field.TextMode == 3 && !isNullOrEmpty(v))
                            sb.appendFormat('<div>{2}<div><a href="javascript:" onclick="var o=$get(\'{0}_Item{1}\');o.style.display=\'block\';o.focus();this.parentNode.parentNode.style.display=\'none\';return false;">{3}</a> | <a href="javascript:" onclick="if(!confirm(\'{5}\'))return;$get(\'{0}_Item{1}\').value=\'\';this.parentNode.parentNode.parentNode.parentNode.style.display=\'none\';return false;">{4}</a></div></div>', this.get_id(), field.Index, this.htmlEncode(field, v).replace(/((\r\n*)|\n)/g, '<br/>'), resourcesData.NoteEditLabel, resourcesData.NoteDeleteLabel, resourcesData.NoteDeleteConfirm);
                    }
                    else {
                        columns = field.Columns > 0 ? field.Columns : 50;
                        var autoCompleteTooltip = field.ToolTip;
                        if (isAutoComplete && (!hasAlias || isEditing)) {
                            v = row[field.Index];
                            sb.appendFormat('<input type="hidden" id="{0}_Item{1}" value="{2}"/>', this.get_id(), field.Index, _app.htmlAttributeEncode(v));
                            field = this._allFields[field.AliasIndex];
                        }
                        if (field.TimeFmtStr)
                            sb.append('<table cellpadding="0" cellspacing="0" class="DateTime"><tr><td class="Date">');
                        sb.appendFormat('<input type="{3}" id="{0}_Item{1}" tabindex="{2}" onchange="$find(&quot;{0}&quot;)._valueChanged({1})" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});"', this.get_id(), field.Index, $nextTabIndex(), field.TextMode != 1 ? 'text' : 'password');
                        if (!isForm)
                            sb.append(' style="display:block;width:90%;"');
                        else
                            sb.appendFormat(' size="{0}"', columns);
                        if (field.Len > 0)
                            sb.appendFormat(' maxlength="{0}"', field.Len);
                        if (isAutoComplete)
                            sb.appendFormat(' title="{0}"', autoCompleteTooltip);
                        v = row[field.Index];
                        if (v == null)
                            s = isAutoComplete ? resourcesData.NullValueInForms : '';
                        else if (hasAlias)
                            s = v.toString();
                        else {
                            if (field.DateFmtStr) {
                                if (typeof (v) == 'string')
                                    v = Date.parseLocale(v, field.DataFormatString.match(/\{0:([\s\S]*?)\}/)[1]);
                                field.DataFormatString = field.DateFmtStr;
                            }
                            s = field.format(v);
                            if (field.DateFmtStr)
                                field.DataFormatString = field.DataFmtStr;
                        }
                        sb.appendFormat(' value="{0}" {1}', _app.htmlAttributeEncode(s), isForm ? ' class="TextInput"' : String.format('class="TextInput {0}Type"', field.Type));
                        sb.appendFormat(' title="{0}"', field.ToolTip);
                        if (_app.supportsPlaceholder && !isNullOrEmpty(field.Watermark))
                            sb.appendFormat(' placeholder="{0}"', _app.htmlAttributeEncode(field.Watermark));
                        sb.append('/>');
                        if (field.Type.startsWith('DateTime') && isForm && resources.Form.ShowCalendarButton && !field.TimeFmtStr)
                            sb.appendFormat('<a id="{0}_Item{1}_Button" href="#" onclick="return false" class="Calendar">&nbsp;</a>', this.get_id(), field.Index);
                        if (field.TimeFmtStr) {
                            sb.appendFormat('</td><td class="Time"><input type="text" id="{0}_Item$Time{1}" tabindex="{2}" onchange="$find(&quot;{0}&quot;)._valueChanged({1})" onfocus="$find(&quot;{0}&quot;)._valueFocused({1});"', this.get_id(), field.Index, $nextTabIndex());
                            if (!isForm)
                                sb.append(' style="display:block;width:90%;"');
                            else
                                sb.appendFormat(' size="{0}"', columns);
                            if (v == null)
                                s = isAutoComplete ? resourcesData.NullValueInForms : '';
                            else if (hasAlias)
                                s = v.toString();
                            else {
                                field.DataFormatString = field.TimeFmtStr;
                                s = field.format(v);
                                field.DataFormatString = field.DataFmtStr;
                            }
                            sb.appendFormat(' value="{0}" {1}/></td></table>', _app.htmlAttributeEncode(s), isForm ? '' : 'class="TimeType"');
                        }
                    }
                if (field._LEV != null) {
                    var lev = this._allFields[field.AliasIndex]._LEV;
                    if (lev == null) lev = '';
                    sb.appendFormat('</td><td class="UseLEV"><a href="javascript:" onclick="$find(\'{0}\')._applyLEV({1});return false" tabindex="{2}" title="{3}">&nbsp;</a></td></tr></table>', this.get_id(), field.Index, $nextTabIndex(), _app.htmlAttributeEncode(String.format(resourcesData.UseLEV, this._allFields[field.AliasIndex].format(lev))));
                }
                if (this._lastCommandName == 'BatchEdit')
                    sb.appendFormat('<div class="BatchSelect"><table cellpadding="0" cellspacing="0"><tr><td><input type="checkbox" id="{0}$BatchSelect{1}" onclick="$app._updateBatchSelectStatus(this,{3})"{4}/></td><td><label for="{0}$BatchSelect{1}">{2}</a></td></tr></table></div>',
                        this.get_id(), batchEditField.Index, resourcesData.BatchUpdate, isForm == true, this._batchEdit && Array.contains(this._batchEdit, batchEditField.Name) ? ' checked="checked"' : '');
            }
            else {
                if (field.OnDemand) this._renderOnDemandItem(sb, field, row, isSelected, isForm);
                else {
                    v = this.htmlEncode(field, row[field.AliasIndex]);
                    if (isEditing)
                        if (readOnly && isSelected) {
                            var hv = row[field.Index]; //row[field.ReadOnly ? field.AliasIndex : field.Index];
                            sb.appendFormat('<input type="hidden" id="{0}_Item{1}" value="{2}"/>', this.get_id(), field.Index, hv != null ? _app.htmlAttributeEncode(this._allFields[field.AliasIndex].format(hv)) : '');
                        }
                    var fieldItems = field.DynamicItems ? field.DynamicItems : field.Items;
                    if (fieldItems.length == 0) {
                        if (field.Type == 'String' && v != null && v.length > resourcesData.MaxReadOnlyStringLen && !(field.TextMode == 3 || field.TextMode == 2))
                            v = v.substring(0, resourcesData.MaxReadOnlyStringLen) + '...';
                        if (v && field.TextMode == 3)
                            v = v.replace(/\r?\n/g, '<br/>');
                        s = isBlank(v) ? (isForm ? resourcesData.NullValueInForms : resourcesData.NullValue) : (this._allFields[field.AliasIndex].format(v));
                    }
                    else if (isCheckBoxList) {
                        lov = v ? v.split(',') : [];
                        var fi = true;
                        for (i = 0; i < fieldItems.length; i++) {
                            item = fieldItems[i];
                            itemValue = item[0] == null ? '' : item[0].toString();
                            if (Array.contains(lov, itemValue)) {
                                if (fi) fi = false; else sb.append(', ');
                                sb.append(_app.htmlEncode(item[1]));
                            }
                        }
                        s = lov.length == 0 ? resourcesData.NullValueInForms : '';
                    }
                    else if (hasAlias && !isSelected)
                        s = v;
                    else {
                        item = this._findItemByValue(field, !hasAlias ? v : row[field.Index]);
                        s = item[1];
                        if (!isForm && s == resourcesData.NullValueInForms)
                            s = resourcesData.NullValue;
                    }
                    if (!isNullOrEmpty(field.HyperlinkFormatString) && v != null) {
                        var location = this._parseLocation(field.HyperlinkFormatString, row);
                        //var m = location.match(_app.LocationRegex);
                        //s = m ? String.format('<a href="javascript:" onclick="_app._navigated=true;window.open(\'{0}\', \'{1}\');return false;">{2}</a>', m[2].replace(/\'/g, '\\\'').replace(/"/g, '&quot;'), m[1], s) : String.format('<a href="{0}" onclick="_app._navigated=true;">{1}</a>', location, s);
                        if (location && location.match(/^mailto\:/))
                            s = String.format('<a href="{0}" title="{2}">{1}</a>', location, s, field.ToolTip);
                        else if (location.match(/^javascript\:/))
                            s = String.format('<a href="javascript:" onclick="{0};return false;" title="{2}">{1}</a>', location.substring(11), s, field.ToolTip);
                        else
                            s = String.format('<a href="javascript:" onclick="$find(\'{2}\')._navigate(\'{0}\');" title="{3}">{1}</a>', location, s, this.get_id(), field.ToolTip);
                    }
                    if (field.TextMode == 1) s = '**********';
                    if (field.Type == 'Byte[]' && !field.OnDemand)
                        s = _app.toHexString(s);
                    if (trimLongWords == true)
                        s = String.trimLongWords(s);
                    if (__designer() && typeof s == 'string')
                        s = s.replace(/\^\w+\^/g, '');
                    sb.append(s);
                }
            }
            if (needObjectRef) {
                if (!isForm) sb.append('</td><td align="right">');
                sb.appendFormat('<span class="ObjectRef" title="{0}" onclick="$find(&quot;{1}&quot;).executeCommand({{commandName: &quot;_ViewDetails&quot;, commandArgument: &quot;{2}&quot;}})">&nbsp;</span>',
                    String.format(resources.Lookup.DetailsToolTip, _app.htmlAttributeEncode(this._allFields[field.AliasIndex].HeaderText)), this.get_id(), field.Name);
                if (!isForm) sb.append('</td></tr></table>');
            }
            if (isForm) {
                if (checkBox == null || this._numberOfColumns > 0)
                    sb.append('</div>');
                //if (this._numberOfColumns > 0) sb.append(errorHtml);
                sb.append(errorHtml);
                if (!isNullOrEmpty(field.FooterText))
                    sb.appendFormat('<div class="Footer">{0}</div>', field.FooterText);
                sb.append('</div>');
            }
        },
        _renderOnDemandItem: function (sb, field, row, isSelected, isForm) {
            var v = row[field.Index];
            var m = v != null ? v.toString().match(/^null\|(.+)$/) : null;
            var isNull = m != null || v == null;
            if (m) v = m[1];
            if (isNull && !isSelected && field.OnDemandStyle == 1)
                sb.append(isForm ? resourcesData.NullValueInForms : resourcesData.NullValue);
            else {
                var blobHref = this.resolveClientUrl(this.get_appRootPath() + resourcesData.BlobHandler);
                if (isSelected && !isNull) sb.appendFormat('<a href="{0}?{1}=o|{2}" title="{3}" onclick="$find(&quot;{4}&quot;)._showDownloadProgress();open(this.href, &quot;_blank&quot;,&quot;height={5},width={6},resizable=yes&quot;);return false">', blobHref, field.OnDemandHandler, v, resourcesData.BlobDownloadHint, this.get_id(), $window.height() / 2, $window.width() / 2);
                if (field.OnDemandStyle == 1) {
                    if (isNull)
                        sb.append(isForm ? resourcesData.NullValueInForms : resourcesData.NullValue);
                    else
                        sb.append(isSelected ? resourcesData.BlobDownloadLink : resourcesData.BlobDownloadLinkReadOnly);
                }
                else {
                    if (!isNull)
                        sb.appendFormat('<img src="{0}?{1}=t|{2}" class="Thumbnail"/>', blobHref, field.OnDemandHandler, v);
                    else
                        sb.append(isForm ? resourcesData.NullValueInForms : resourcesData.NullValue);
                }
                if (isSelected && !isNull) sb.append('</a>');
                if (!field.ReadOnly && (this.editing() && isSelected))
                    if (_app.upload())
                        sb.appendFormat('<div class="drop-box-{0}"></div>', field.Index);
                    else
                        sb.appendFormat('<iframe src="{0}?{1}=u|{2}&owner={3}&index={4}" frameborder="0" scrolling="no" id="{3}_Frame{4}" tabindex="{6}"></iframe><div style="display:none" id="{3}_ProgressBar{4}" class="UploadProgressBar">{5}</div>', blobHref, field.OnDemandHandler, v, this.get_id(), field.Index, 'Uploading...', $nextTabIndex());
            }
        },
        _showUploadProgress: function (index, blobForm) {
            var f = $get(String.format('{0}_Frame{1}', this.get_id(), index));
            var p = $get(String.format('{0}_ProgressBar{1}', this.get_id(), index));
            if (f != null && p != null) {
                var inputFile = $(blobForm).find('input:file');
                var fileName = inputFile.val().split(/(\\|\/)/);
                fileName = fileName.length > 0 ? fileName[fileName.length - 1] : null;
                var inserting = this.inserting();
                var $p = $(p).show().text(fileName).focus();
                var padding = $common.getPaddingBox(p);
                var border = $common.getBorderBox(p);
                p.style.width = (f.offsetWidth - padding.horizontal - border.horizontal) + 'px';
                p.style.height = (f.offsetHeight - padding.vertical - border.vertical) + 'px';
                //Sys.UI.DomElement.setVisible(f, false);
                var $f = $(f).hide();
                if (!inserting) {
                    this._showDownloadProgress();
                    blobForm.submit();
                }
                else {
                    var pendingUploads = this._pendingUploads;
                    if (!pendingUploads)
                        pendingUploads = this._pendingUploads = [];
                    pendingUploads.push({ form: blobForm, progress: p });
                    $p.find('a').button('destroy');
                    $p.addClass('dataview-insert').append(
                        $(String.format('<a>{0}</a>', resourcesDataFiltersLabels.Clear)).button({ icons: { primary: "ui-icon-close" }, text: false }).click(function () {
                            $p.hide();
                            $f.show();
                            for (var i = 0; i < pendingUploads.length; i++)
                                if (pendingUploads[i].form == blobForm) {
                                    Array.removeAt(pendingUploads, i);
                                    break;
                                }
                            inputFile.replaceWith(inputFile.clone(true));
                        })
                    );
                }
            }
        },
        _internalRenderActionButtons: function (sb, location, scope, actions) {
            if (!actions)
                actions = this._actionButtonsInScope(scope);
            this._clonedRow = this._cloneChangedRow();

            for (var i = 0; i < actions.length; i++) {
                action = actions[i];
                if (this._isActionAvailable(action)) {
                    var className = !isNullOrEmpty(action.CssClass) ? action.CssClass : '';
                    if (action.HeaderText && action.HeaderText.length > 10) {
                        if (className.length > 0) className += ' ';
                        className += 'AutoWidth';
                    }
                    var disabled = action.CommandName == 'None';
                    sb.appendFormat('<button onclick="{6}$find(\'{0}\').executeAction(\'{5}\', {1},-1);return false" tabindex="{3}" class="{8}{4}"{7}>{2}</button>',
                        this.get_id(), i, action.HeaderText, $nextTabIndex(),
                        className,
                        scope, disabled ? 'return false;' : '', disabled ? ' disabled="disabled"' : '',
                        action.CssClassEx);
                }
                //if (action._whenClientScript != null)
                if (action.WhenClientScript)
                    this._dynamicActionButtons = true;
            }

            this._clonedRow = null;
        },
        _actionButtonsInScope: function (scope) {
            var actions = this.get_actions(scope);
            if (actions.length == 0) return;
            if (scope == 'Row') {
                var show = false;
                for (var i = 0; i < actions.length; i++) {
                    if (this._isActionAvailable(actions[i])) {
                        show = true;
                        break;
                    }
                }
                if (!show) return null;
            }
            return actions;
        },
        _renderActionButtons: function (sb, location, scope) {
            if (this.get_showActionButtons().indexOf(location) == -1) return;
            var actions = this._actionButtonsInScope(scope);
            if (!actions) return;
            var colSpan = this._get_colSpan();
            var actionColumnTd = '';
            if (scope == 'Row' && this._actionColumn) {
                colSpan--;
                actionColumnTd = '<td class="ActionColumn">&nbsp;</td>';
            }
            sb.appendFormat('<tr class="ActionButtonsRow {0}ButtonsRow {2}Scope">{3}<td colspan="{1}" class="ActionButtons {2}ActionButtons">', location, colSpan, scope, actionColumnTd);

            sb.append('<table style="width:100%" cellpadding="0" cellspacing="0" class="ActionButtons"><tr>');

            var actionButtonsId = String.format(' id="{0}$ActionButtons${1}"', this.get_id(), location);

            if (scope == 'Form') {
                var p = this._position;
                var allowNav = p && p.count > 1 && !this.inserting();
                var allowPrevious = p && p.index > 0;
                var allowNext = p && p.index < p.count - 1;
                var printAction = this._get_specialAction('Print');
                var annotateAction = this.get_isModal() ? this._get_specialAction('Annotate') : null;
                sb.appendFormat('<td class="Left"><table class="FormNav"><tr><td class="Previous{5}{7}"><a href="javascript:" onclick="$find(\'{2}\')._advance(-1);return false;" title="{3}"><span></span></a></td><td class="Next{6}{7}"><a href="javascript:" onclick="$find(\'{2}\')._advance(1);return false;" title="{4}"><span></span></a></td><td class="Print{9}"><a href="javascript:" onclick="{10};return false;" title="{8}"><span></span></a></td><td class="Annotate{12}"><a href="javascript:" onclick="{13};return false;" title="{11}"><span></span></a></td><td class="Instruction" id="{0}_Wait" align="left">{1}</td></tr></table></td><td class="Right" align="right"{14}>&nbsp;',
                    location == 'Bottom' ? this.get_id() : '',
                    this.editing() && resources.Form.RequiredFieldMarker ? resources.Form.RequiredFiledMarkerFootnote : '',
                    this.get_id(), resourcesPager.Previous, resourcesPager.Next,
                    allowPrevious ? '' : ' Disabled', allowNext ? '' : ' Disabled', allowNav ? '' : ' Hidden',
                    printAction ? printAction.text : '', printAction ? '' : ' Hidden', printAction ? printAction.script : null,
                    annotateAction ? annotateAction.text : '', annotateAction ? '' : ' Hidden', annotateAction ? annotateAction.script : null,
                    actionButtonsId);
            }
            else
                sb.appendFormat('<td{0}>', actionButtonsId);

            this._internalRenderActionButtons(sb, location, scope, actions);
            this._lastActionButtonsScope = scope;

            sb.append('</td></tr></table>');

            sb.append('</td></tr>');
        },
        _isActionMatched: function (action, ignoreScript) {
            //if (!action._csInitialized) {
            //    if (!isNullOrEmpty(action.WhenClientScript))
            //        action._whenClientScript = this._prepareJavaScriptExpressionEx(action.WhenClientScript);
            //    action._csInitialized = true;
            //}
            var whenClientScript = action.WhenClientScript;
            if (typeof whenClientScript === 'string')
                action.WhenClientScript = whenClientScript = whenClientScript ? eval('(function(){return ' + this._prepareJavaScriptExpressionEx(whenClientScript) + '})') : null;

            var result =
                (action.WhenViewRegex == null || (action.WhenViewRegex.exec(this.get_viewId()) != null) == action.WhenViewRegexResult) &&
                (action.WhenTagRegex == null || (action.WhenTagRegex.exec(this.get_tag()) != null) == action.WhenTagRegexResult) &&
                (action.WhenHRefRegex == null || (action.WhenHRefRegex.exec(location.pathname) != null) == action.WhenHRefRegexResult) &&
                //(action._whenClientScript == null || !!eval(action._whenClientScript)/* == true*/);
                (ignoreScript || !whenClientScript || !!whenClientScript.call(this));
            return result;
        },
        _isActionAvailable: function (action, rowIndex) {
            var that = this,
                commandName = action.CommandName,
                whenLastCommandName = action.WhenLastCommandName,
                lastCommand = whenLastCommandName ? whenLastCommandName : '';
            if (lastCommand == 'Any')
                lastCommand = that.get_lastCommandName();
            var lastArgument = action.WhenLastCommandArgument ? action.WhenLastCommandArgument : '';
            var available = lastCommand.length == 0 || (lastCommand == that.get_lastCommandName() && (lastArgument.length == 0 || lastArgument == that.get_lastCommandArgument()));
            if (available) {
                var editing = that.editing();
                if (commandName == 'DataSheet')
                    return !editing && that.get_isGrid() && /*!that.get_isTree() &&*/that.get_viewType() != 'DataSheet' && that._isActionMatched(action);
                else if (commandName == 'Grid')
                    return !editing && that.get_isGrid() && /*!that.get_isTree() &&*/that.get_viewType() != 'Grid' && that._isActionMatched(action);
                else if (commandName == 'BatchEdit')
                    return that.get_showMultipleSelection() && that._selectedKeyList && that._selectedKeyList.length > 1 && that._isActionMatched(action);
                else if (editing) {
                    var isSelected = that._rowIsSelected(rowIndex == null ? that._selectedRowIndex : rowIndex);
                    if (isSelected)
                        return (lastCommand == 'New' || lastCommand == 'Edit' || lastCommand == 'BatchEdit' || lastCommand == 'Duplicate') && that._isActionMatched(action);
                    else if (!isSelected && rowIndex == null && (lastCommand == 'New' || lastCommand == 'Duplicate'))
                        return that._isActionMatched(action);
                    else
                        return lastCommand.length == 0 && rowIndex != null && that._isActionMatched(action);
                }
            }
            return available && (!action.WhenKeySelected || action.WhenKeySelected && that._selectedKey && that._selectedKey.length > 0) && that._isActionMatched(action) && (commandName != 'New' || that._hasKey());
        },
        _hasKey: function () { return this._keyFields && this._keyFields.length > 0; },
        _rowIsSelected: function (rowIndex) {
            var that = this,
                result = that._rowIsSelectedCached;
            if (result != null)
                return result;
            if (!that._hasKey()) return that.get_isModal() && that.get_isForm()/* this.get_view().Type == 'Form'*/;
            var row = that._rows[rowIndex];
            return that.rowIsSelected(row);
        },
        rowIsSelected: function (row) {
            var that = this,
                keyFields = that._keyFields,
                selectedKey = that._selectedKey;
            if (row && keyFields.length == selectedKey.length && keyFields.length) {
                if (row == that._mergedRow) return true;
                for (var j = 0; j < keyFields.length; j++) {
                    var field = keyFields[j],
                        v1 = selectedKey[j],
                        v2 = row[field.Index];
                    //if (v1 === 0) 
                    //    return false;
                    if (field.Type.startsWith('DateTime')) {
                        if (!(v1 || v2)) return false;
                        v1 = v1.toString();
                        v2 = v2.toString();
                    }
                    if (v1 != v2 && !(v2 == null && isNullOrEmpty(v1))) return false;
                }
                return true;
            }
            else
                return that._inlineEditor ? true : false;
        },
        rowIsTemplate: function (row) {
            if (!row)
                row = this.extension().commandRow();
            var keyFields = this._keyFields,
                selectedKey = this._selectedKey,
                result = true;
            if (row && row.length && keyFields.length)
                for (var j = 0; j < keyFields.length; j++) {
                    var field = keyFields[j];
                    if (row[field.Index] != null) {
                        result = false;
                        break;
                    }
                }
            else
                result = false;
            return result;
        },
        _get_template: function (type) {
            if (this.get_isDataSheet()) return null;
            return $get(this.get_controller() + '_' + this.get_viewId() + (type ? '_' + type : ''));
        },
        _renderTemplate: function (template, sb, row, isSelected, isInlineForm) {
            var s = typeof template == 'string' ? template : template.innerHTML;
            var iterator = /([\s\S]*?)\{([\w\d]+)(\:([\S\s]+?)){0,1}\}/g;
            var lastIndex = 0;
            var match = iterator.exec(s);
            while (match) {
                lastIndex = match.index + match[0].length;
                sb.append(match[1]);
                var field = this.findField(match[2]);
                if (field) {
                    if (match[4] && match[4].length > 0)
                        sb.appendFormat('{0:' + match[4] + '}', row[field.Index]);
                    else
                        this._renderItem(sb, field, row, isSelected, isInlineForm, null, null, null, true);
                }
                else {
                    var dataView = _app.find(match[2]);
                    if (dataView) {
                        if (!this._embeddedViews)
                            this._embeddedViews = [];
                        Array.add(this._embeddedViews, { 'view': dataView });
                        sb.appendFormat('<div id="v_{0}" class="EmbeddedViewPlaceholder"></div>', dataView.get_id());
                    }
                }

                match = iterator.exec(s);
            }
            if (lastIndex < s.length) sb.append(s.substring(lastIndex));
        },
        _renderNewRow: function (sb) {
            if (this.inserting()) {
                var isDataSheet = this.get_isDataSheet();
                var hasActionColumn = this._actionColumn && !isDataSheet;
                var cell = this._get_focusedCell();
                if (!cell) cell = { colIndex: 0 };

                var t = isDataSheet ? null : this._gridTemplates.New; //this._get_template('new');
                sb.appendFormat('<tr class="Row Selected{0}{1}">', t ? ' InlineFormRow' : '', isDataSheet ? ' Inserting' : '');

                var multipleSelection = this.multiSelect();
                if (multipleSelection) sb.append('<td class="Cell Toggle First">&nbsp;</td>');
                var showIcons = this.get_showIcons();
                if (showIcons) sb.appendFormat('<td class="Cell Icons{0}"><span>&nbsp;</span></td>', !multipleSelection ? ' First' : '');
                if (this.get_isDataSheet()) sb.appendFormat('<td class="Cell Gap"><div class="Icon"></div></td>', !multipleSelection && !showIcons ? ' First' : '');
                if (hasActionColumn)
                    sb.append('<td class="Cell ActionColumn">&nbsp;</td>');
                var row = this._newRow ? this._newRow : [];
                this._mergeRowUpdates(row);
                this._updateVisibility(row);
                if (t) {
                    sb.appendFormat('<td class="Cell" colspan="{0}">', this.get_fields().length);
                    this._renderTemplate(t, sb, row, true, true);
                    sb.append('</td>');
                }
                else {
                    for (var i = 0; i < this._fields.length; i++) {
                        var field = this._fields[i];
                        var af = this._allFields[field.AliasIndex];
                        var cellEvents = '';

                        if (isDataSheet) {
                            this._editing = cell && cell.colIndex == field.ColIndex;
                            if (!this._editing)
                                cellEvents = String.format(' onclick="$find(\'{0}\')._dataSheetCellFocus(event,-1,{1})"', this.get_id(), i);
                        }
                        sb.appendFormat('<td class="Cell {0} {1}Type{2}{3}"{4}>', af.Name, af.Type, i == 0 ? ' FirstColumn' : '', i == this._fields.length - 1 ? ' LastColumn' : '', cellEvents)
                        if (!field.ReadOnly) sb.appendFormat('<div class="Error" id="{0}_Item{1}_Error" style="display:none"></div>', this.get_id(), field.Index);

                        this._renderItem(sb, field, row, !field.OnDemand, null);
                        this._editing = null;
                        sb.append('</td>');
                    }
                }
                sb.append('</tr>');
                if (!isDataSheet)
                    this._renderActionButtons(sb, 'Bottom', 'Row')
            }
        },
        _iconClicked: function (rowIndex) {
            if (!this.get_isDataSheet() && this.get_lookupField() == null)
                if (this._icons && this._icons[rowIndex] == 'Attachment') {
                    this._lastClickedIcon = 'Attachment';
                    this.set_autoSelectFirstRow(true);
                    this._autoSelect(rowIndex);
                }
        },
        _renderRows: function (sb, hasKey, multipleSelection) {
            var isInLookupMode = this.get_lookupField() != null;
            var isDataSheet = this.get_isDataSheet();
            var hasActionColumn = !isInLookupMode && this._actionColumn && !isDataSheet;
            var actionColumnGroup = hasActionColumn ? this.get_actionGroups('ActionColumn', true) : null;
            var expressions = this._enumerateExpressions(Web.DynamicExpressionType.Any, Web.DynamicExpressionScope.ViewRowStyle, this.get_viewId());
            sb.append('<tr class="HeaderRow">');
            var showIcons = this.get_showIcons();
            if (multipleSelection) {
                sb.appendFormat('<th class="Toggle First"><input type="checkbox" onclick="$find(&quot;{0}&quot;).toggleSelectedRow()" id="{0}_ToggleButton"/></th>', this.get_id());
                this._multipleSelection = false;
            }
            if (showIcons) sb.appendFormat('<th class="Icons{0}">&nbsp;</th>', !multipleSelection ? ' First' : '');
            if (isDataSheet) sb.appendFormat('<th class="Gap{0}">&nbsp;</th>', !multipleSelection && !showIcons ? ' First' : '');
            if (hasActionColumn) sb.appendFormat('<th class="FieldHeaderSelector ActionColumn"><span>{0}</span></th>', this._actionColumn);
            var sortExpression = this.get_sortExpression(),
                sortExprInfo = sortExpression ? sortExpression.match(/^\s*(\w+)((\s+(asc|desc))|(\s*(,|$)))/i) : null;
            for (var i = 0; i < this._fields.length; i++) {
                var field = this._fields[i],
                    originalFieldName = field.Name;
                field = this._allFields[field.AliasIndex];
                if (field.Name == originalFieldName)
                    originalFieldName = '';
                sb.appendFormat('<th class="FieldHeaderSelector {4} {0} {1}Type{2}{3}"', field.Name, field.Items.length > 0 ? 'String' : field.Type, i == 0 ? ' FirstColumn' : '', i == this._fields.length - 1 ? ' LastColumn' : '', originalFieldName);
                if (field.AllowSorting || field.AllowQBE)
                    sb.appendFormat(' onmouseover="$showHover(this,\'{0}$FieldHeaderSelector${1}\',\'FieldHeaderSelector\')" onmouseout="$hideHover(this)" onclick="$toggleHover()"', this.get_id(), i);
                sb.append('>');
                if (field.AllowSorting) {
                    sb.appendFormat('<a href="#" onclick="$find(\'{0}\').sort(\'{1}\');$preventToggleHover();return false" title="{3}" onfocus="$showHover(this,\'{0}$FieldHeaderSelector${4}\',\'FieldHeaderSelector\',1)" onblur="$hideHover(this)" tabindex="{5}">{2}</a>',
                        this.get_id(), field.Name, field.HeaderText, String.format(resourcesHeaderFilter.SortBy, field.HeaderText), i, $nextTabIndex());
                    if (sortExprInfo && sortExprInfo[1] == field.Name)
                        sb.append(String.format('<span class="{0}">&nbsp;</span>', sortExprInfo[4] ? (sortExprInfo[4].toLowerCase() == 'asc' ? 'SortUp' : 'SortDown') : 'SortUp'));
                    if (this.filterOf(field) != null)
                        sb.append('<span class="Filter">&nbsp;</span>');
                }
                else
                    sb.appendFormat('<span>{0}</span>', field.HeaderText);
                sb.append('</th>');
            }
            sb.append('</tr>');
            var cell = this._get_focusedCell();
            var isEditing = this.editing();
            var isInserting = this.inserting();
            var newRowIndex = this._lastSelectedRowIndex;
            if (!this._gridTemplates) {
                var gt = { 'Default': this._get_template(), 'Edit': this._get_template('edit'), 'New': this._get_template('new') };
                if (gt.Default)
                    gt.Default = gt.Default.innerHTML;
                gt.Edit = gt.Edit ? gt.Edit.innerHTML : gt.Default;
                gt.New = gt.New ? gt.New.innerHTML : gt.Default;
                this._gridTemplates = gt;
            }
            var t = isDataSheet ? null : this._gridTemplates.Edit; //isEditing ? this._get_template('edit') : null;
            var ts = isDataSheet ? null : this._gridTemplates.Default;
            var family = null;
            this._registerRowSelectorItems();
            var mouseOverEvents = 'onmouseover="$(this).addClass(\'Highlight\');" onmouseout="$(this).removeClass(\'Highlight\')"';
            var showRowNumber = this.get_showRowNumber();
            var hasSelectedRow = false;
            for (i = 0; i < this.get_rows().length; i++) {
                var row = this.get_rows()[i];
                var customCssClasses = ' ' + this._evaluateJavaScriptExpressions(expressions, row, true);
                var isSelectedRow = this._rowIsSelected(i);
                if (isSelectedRow)
                    hasSelectedRow = true;
                if (isSelectedRow) this._selectedRowIndex = i;
                var checkBoxCell = null;
                var multiSelectedRowClass = '';
                if (multipleSelection) {
                    var selected = Array.indexOf(this._selectedKeyList, this._createRowKey(i)) != -1;
                    if (selected) this._multipleSelection = true;
                    checkBoxCell = String.format('<td class="Cell Toggle First"><input type="checkbox" id="{0}_CheckBox{1}" onclick="$find(&quot;{0}&quot;).toggleSelectedRow({1})"{2} class="MultiSelect{3}"/></td>', this.get_id(), i, selected ? ' checked="checked"' : null, selected ? ' Selected' : '');
                    if (selected) multiSelectedRowClass = ' MultiSelectedRow';
                }
                var iconCell = showIcons ? String.format('<td class="Cell Icons {0}{1}"><span onclick="$find(\'{3}\')._iconClicked({4});">{2}</span></td>', this._icons ? this._icons[i] : '', !multipleSelection ? ' First' : '', showRowNumber ? this.get_pageSize() * this.get_pageIndex() + i + 1 + (this._pageOffset ? this._pageOffset : 0) : '&nbsp;', this.get_id(), i) : '';
                if (isDataSheet)
                    iconCell += String.format('<td class="Cell Gap{2}" onclick="$find(\'{0}\')._dataSheetCellFocus(event,{1},-1)"><div class="Icon"></div></td>', this.get_id(), i, !multipleSelection && !showIcons ? ' First' : '');
                if (hasActionColumn)
                    iconCell += this._renderActionColumnCell(row, i, isSelectedRow, actionColumnGroup)
                if (isEditing && isSelectedRow) {
                    this._mergeRowUpdates(row);
                    this._updateVisibility(row);
                }
                if (isSelectedRow && (isEditing && t || ts)) {
                    sb.appendFormat('<tr id="{0}_Row{1}" class="{2}Row{3} Selected{7}">{5}{6}<td class="Cell" colspan="{4}">', this.get_id(), i, i % 2 == 0 ? '' : 'Alternating', ' InlineFormRow', this.get_fields().length, checkBoxCell, iconCell, isEditing ? ' Editing' : '');
                    this._renderTemplate(isEditing && t ? t : ts, sb, row, true, true);
                    sb.append('</td>');
                }
                else {
                    sb.appendFormat('<tr id="{0}_Row{1}" class="{2}Row{3}{4}{7}" {6}>', this.get_id(), i, i % 2 == 0 ? '' : 'Alternating', isSelectedRow ? ' Selected' + customCssClasses : customCssClasses, multiSelectedRowClass, hasKey ? '' : ' ReadOnlyRow', isDataSheet && !isInLookupMode ? '' : mouseOverEvents, isSelectedRow && isEditing ? ' Editing' : ''/*,
                    !isEditing && isDataSheet ? String.format(' onmousewheel="$find(\'{0}\')._scrollToRow(event.wheelDelta);return false;"', this.get_id()) : ''*/);
                    if (checkBoxCell) sb.append(checkBoxCell);
                    sb.append(iconCell);
                    for (j = 0; j < this._fields.length; j++) {
                        field = this._fields[j];
                        var af = this._allFields[field.AliasIndex];
                        originalFieldName = field.Name == af.Name ? '' : field.Name;
                        if (cell)
                            this._editing = isEditing && cell.rowIndex == i && cell.colIndex == field.ColIndex;
                        var allowRowSelector = j == 0 && hasKey;
                        if (allowRowSelector) {
                            family = _web.HoverMonitor.Families[String.format('{0}$RowSelector${1}', this.get_id(), i)];
                            if (!family || family.items.length == 0)
                                allowRowSelector = false;
                        }
                        var firstColumnClass = j == 0 ? ' FirstColumn' : '';
                        var cellClickEvent = String.format(' onclick="$find(\'{0}\')._{3}CellFocus(event,{1},{2})"', this.get_id(), i, j, isDataSheet && !isInLookupMode ? 'dataSheet' : 'gridView');
                        //if (isDataSheet)
                        //    cellClickEvent += String.format(' onclick="$find(\'{0}\')._dataSheetCellEdit(event,{1},{2})"', this.get_id(), i, j, isDataSheet ? 'dataSheet' : 'gridView');
                        var lastColumnClass = j == this._fields.length - 1 ? ' LastColumn' : '';
                        if (allowRowSelector && !isInLookupMode || isSelectedRow && isEditing || field.OnDemand && isSelectedRow)
                            sb.appendFormat('<td class="Cell {5} {0} {1}Type{2}{4}"{3}>', af.Name, field.Items.length > 0 ? 'String' : af.Type, firstColumnClass, isSelectedRow && isEditing && (!isDataSheet || cell && cell.colIndex == field.ColIndex) ? '' : cellClickEvent, lastColumnClass, originalFieldName);
                        else
                            sb.appendFormat('<td class="Cell {7} {2} {3}Type{4}{6}" style="cursor:default;"{5}>', this.get_id(), i, af.Name, af.Type == 'Byte[]' ? 'Binary' : (field.Items.length > 0 ? 'String' : af.Type), firstColumnClass, cellClickEvent, lastColumnClass, originalFieldName);
                        if (isSelectedRow && isEditing && !field.ReadOnly) sb.appendFormat('<div class="Error" id="{0}_Item{1}_Error" style="display:none"></div>', this.get_id(), field.Index);
                        if (allowRowSelector) {
                            //var family = Web.HoverMonitor.Families[String.format('{0}$RowSelector${1}', this.get_id(), i)];
                            if (!isInLookupMode && family && family.items.length > 1)
                                sb.appendFormat('<div id="{0}_RowSelector{1}" class="RowSelector" onmouseover="$showHover(this, \'{0}$RowSelector${1}\', \'RowSelector\')" onmouseout="$hideHover(this)" onclick="$toggleHover()">', this.get_id(), i);
                            if (!(isSelectedRow && isEditing)) {
                                var focusEvents = isInLookupMode || !family || family.items.length == 1 ? '' : String.format(' onfocus="$showHover(this, \'{0}$RowSelector${1}\', \'RowSelector\', 1)" onblur="$hideHover(this)" ', this.get_id(), i);
                                if (!isInLookupMode) sb.appendFormat('<a href="#" onclick="$hoverOver(this, 2);$find(\'{0}\').executeAction(\'Grid\',-1,{1});$preventToggleHover();return false" tabindex="{2}"{3}>', this.get_id(), i, $nextTabIndex(), focusEvents); else sb.appendFormat('<a href="javascript:" onclick="return false" tabindex="{0}">', $nextTabIndex());
                            }
                        }
                        this._renderItem(sb, field, row, isSelectedRow, null, allowRowSelector);
                        if (allowRowSelector && !isEditing) {
                            if (!(isSelectedRow && isEditing)) sb.append('</a>');
                            if (!isInLookupMode && family && family.items.length > 1) sb.append('</div>');
                        }
                        sb.append('</td>');
                        if (cell)
                            this._editing = null;
                    }
                }
                sb.append('</tr>');
                if (isSelectedRow && cell == null)
                    this._renderActionButtons(sb, 'Bottom', 'Row');
                if (isInserting && newRowIndex == i) {
                    newRowIndex = -2;
                    this._renderNewRow(sb);
                }
                if (this._syncFocusedCell && cell && isSelectedRow)
                    cell.rowIndex = i;
            }
            if (isInserting && newRowIndex != -2) this._renderNewRow(sb);
            if (this._saveAndNew) {
                this._saveAndNew = false;
                if (this._syncFocusedCell)
                    this.newDataSheetRow();
                else {
                    cell = this._get_focusedCell();
                    if (cell) {
                        cell.colIndex = 0;
                        this._moveFocusToNextRow(cell, this.get_pageSize());
                    }
                }
            }
            this._syncFocusedCell = false;
            if (!hasSelectedRow)
                this._selectedRowIndex = null;
        },
        _renderActionColumnCell: function (row, rowIndex, isSelectedRow, actionGroups) {
            this._clonedRow = row;
            if (!isSelectedRow) {
                var lastCommandName = this.get_lastCommandName();
                var lastCommandArgument = this.get_lastCommandArgument();
                this.set_lastCommandName('Select')
                this.set_lastCommandArgument(null);
            }
            var sb = new Sys.StringBuilder();
            sb.append('<td class="Cell ActionColumn">');
            //sb.append('Edit | Del');
            var first = true;
            for (var i = 0; i < actionGroups.length; i++) {
                var ag = actionGroups[i];
                for (var j = 0; j < ag.Actions.length; j++) {
                    var a = ag.Actions[j];
                    if (this._isActionAvailable(a)) {
                        if (first)
                            first = false;
                        else
                            sb.append('<span class="Divider">&nbsp;</span>');
                        //this.executeAction(scope, actionindex, rowindex, groupindex);
                        sb.appendFormat('<a href="javascript:" onclick="var dv=$find(\'{0}\');dv.executeAction(\'ActionColumn\',{2},{4},{3});return false;" class="{6}"><span class="Outer"><span class="Inner"><span class="Self" title="{5}">{1}</span></span></span></a>',
                            this.get_id(), _app.htmlEncode(a.HeaderText), j, i, rowIndex, _app.htmlEncode(a.Description), a.CssClass);
                    }
                }
            }
            if (first)
                sb.append('&nbsp;');
            sb.append('</td>');
            var result = sb.toString();
            sb.clear();
            this._clonedRow = null;
            if (!isSelectedRow) {
                this.set_lastCommandName(lastCommandName);
                this.set_lastCommandArgument(lastCommandArgument);
            }
            return result;
        },
        extension: function () {
            var result = null,
                that = this,
                wdve = _app.Extensions;
            if (wdve) {
                var viewType = that.get_viewType(),
                    altViewType = that._altViewType,
                    viewExtensions = that._extensions;
                if (viewExtensions == null)
                    viewExtensions = that._extensions = {};
                if (viewType && altViewType == null)
                    altViewType = that._altViewType = that.tagged('view-type-inline-editor') ? 'Form' : '';
                if (altViewType)
                    viewType = altViewType;
                result = viewExtensions[viewType];
                if (result == null) {
                    var extensionType = wdve ? wdve[viewType] : null;
                    result = that._extensions[viewType] = extensionType ? new extensionType(this) : 0;
                    if (result)
                        result.initialize();
                }
            }
            return result;
        },
        _renderGridView: function (sb) {
            this._renderViewDescription(sb);
            this._renderActionBar(sb);
            this._renderSearchBar(sb);
            var pagerLocation = this.get_showPager();
            var dataViewExtension = this.extension();
            this._renderPager(sb, 'Top');
            if (this.get_isChart()) {
                this._renderInfoBar(sb);
                this._sortingDisabled = false;
                var sorted = false;
                for (var i = 0; i < this._fields.length; i++) {
                    var f = this._fields[i];
                    if (f.Aggregate != 0)
                        this._sortingDisabled = true;
                    else if (!sorted) {
                        sorted = true;
                        this.set_sortExpression(null);
                        this.set_sortExpression(f.Name);
                    }
                }
                if (!dataViewExtension)
                    sb.appendFormat('<tr class="ChartRow"><td colspan="{1}" class="ChartCell"><img id="{0}$Chart" class="Chart" onload="if(this.readyState==\'complete\'){{this.style.height=\'\';_body_performResize()}}"/></td></tr>', this.get_id(), this._get_colSpan());
            }
            else {
                var hasKey = this._hasKey();
                var multipleSelection = this.multiSelect() && hasKey;
                if (!this.get_searchOnStart()) {
                    this._renderInfoBar(sb);
                    if (!dataViewExtension)
                        this._renderRows(sb, hasKey, multipleSelection);
                }
                if (!dataViewExtension) {
                    this._renderAggregates(sb, multipleSelection);
                    this._renderNoRecordsWhenNeeded(sb);
                }
            }
            if (!dataViewExtension)
                this._renderPager(sb, 'Bottom');
        },
        _renderAggregates: function (sb, multipleSelection) {
            var aggregates = this.get_aggregates();
            if (this._totalRowCount == 0 || aggregates == null) return;
            sb.append('<tr class="AggregateRow">');
            if (multipleSelection) sb.append('<td class="Aggregate">&nbsp;</td>');
            if (this.get_showIcons()) sb.append('<td class="Aggregate">&nbsp;</td>');
            if (this.get_isDataSheet()) sb.append('<td class="Aggregate">&nbsp;</td>');
            if (this._actionColumn) sb.append('<td class="Aggregate">&nbsp;</td>');
            for (var i = 0; i < this.get_fields().length; i++) {
                var field = this.get_fields()[i];
                if (field.Aggregate == 0) sb.append('<td class="None">&nbsp;</td>');
                else {
                    var v = aggregates[field.Index];
                    if (v == null) v = resourcesData.NullValue;
                    else v = field.format(v);
                    var f = this._allFields[field.AliasIndex];
                    if (f.IsMirror)
                        v = aggregates[field.AliasIndex];
                    var a = resources.Grid.Aggregates[Web.DataViewAggregates[field.Aggregate]];
                    sb.appendFormat('<td class="Aggregate {0} {1}Type" title="', f.Name, f.Type);
                    sb.appendFormat(a.ToolTip, f.HeaderText);
                    sb.append('">');
                    sb.appendFormat(a.FmtStr, v);
                    sb.append('</td>');
                }
            }
            sb.append('</tr>');
        },
        _renderSearchBar: function (sb) {
            if (!this.get_showSearchBar()) return;
            if (__tf != 4) return;
            this._searchBarInitialized = false;
            sb.appendFormat('<tr class="SearchBarRow" id="{0}$SearchBar" style="{2}"><td colspan="{1}" class="SearchBarCell" id="{0}$SearchBarContent">Search bar goes here.<br/><br/><br/><br/></td></tr>', this.get_id(), this._get_colSpan(), this.get_searchBarIsVisible() ? '' : 'display:none');
        },
        _renderNoRecordsWhenNeeded: function (sb) {
            if (this._totalRowCount == 0) {
                var newRowLink = this.get_isDataSheet() && this._keyFields.length > 0 && this.executeActionInScope(['Row', 'ActionBar'], 'New', null, null, true) ? String.format(' <a href="javascript:" class="NewRowLink" onclick="$find(\'{0}\').newDataSheetRow();return false;" title="{2}">{1}</a>', this.get_id(), resources.Grid.NewRowLink, resources.Lookup.GenericNewToolTip) : '';
                sb.appendFormat('<tr class="Row NoRecords"><td colspan="{0}" class="Cell">{1}{2}</td></tr>', this._get_colSpan(), resourcesData.NoRecords, newRowLink);
            }
        },
        _attachBehaviors: function () {
            if (_touch) return;
            this._detachBehaviors();
            this._attachFieldBehaviors();
            var e = this.get_quickFindElement();
            if (e) $addHandlers(e, this._quickFindHandlers, this);
        },
        _get: function (family, index) {
            return index == null ? $get(this.get_id() + family) : $get(this.get_id() + family + index);
        },
        _attachTimeOptions: function (field, element) {
            var c = $create(Web.AutoComplete, {
                'completionInterval': 500,
                'contextKey': '', //String.format('{0},{1},{2}', this.get_controller(), this.get_viewId(), field.Name),
                'delimiterCharacters': '',
                'id': this.get_id() + '_AutoComplete$Time' + field.Index,
                'minimumPrefixLength': field.AutoCompletePrefixLength,
                //'serviceMethod': 'GetCompletionList',
                //'servicePath': this.get_servicePath(),
                //'useContextKey': true,
                'typeCssClass': 'AutoComplete'
            },
                null, null, element);
            Sys.UI.DomElement.addCssClass(c._completionListElement, 'Time');
            var cache = null,
                cacheKey = field.DataFormatString && field.DataFormatString.replace(/\W/g, '_') || 'Default';
            if (field.Type == 'TimeSpan') {
                if (!_app._timeSpanOptions)
                    _app._timeSpanOptions = {};
                cache = _app._timeSpanOptions[cacheKey]
                if (!cache) {
                    cache = [];
                    var dt = new Date(),
                        formatString = field.DataFormatString || '{0:HH:mm}';
                    dt.setHours(0, 0, 0, 0);
                    while (cache.length < 24 * 4) {
                        Array.add(cache, String.localeFormat(formatString, dt)/*String.format('{0:d2}{1}{2:d2}', dt.getHours(), dateTimeFormat.TimeSeparator, dt.getMinutes())*/);
                        dt.setMinutes(dt.getMinutes() + 15);
                    }
                    _app._timeSpanOptions[cacheKey] = cache;
                }
            }
            else {
                cache = _app._timeOptions;
                if (!cache) {
                    cache = [];
                    dt = new Date();
                    dt.setHours(dt.getHours(), 0, 0, 0);
                    while (cache.length < 24 * 2) {
                        Array.add(cache, String.localeFormat('{0:' + dateTimeFormat.ShortTimePattern + '}', dt));
                        dt.setMinutes(dt.getMinutes() + 30);
                    }
                    _app._timeOptions = cache;
                }
            }
            (c._cache = [])['%'] = cache;
            return c;
        },
        _executeClientEditorFactories: function (field, element, attach) {
            var editors = element == null ? field.Editor : field.ClientEditor;
            if (!isNullOrEmpty(editors)) {
                var factories = editors.split(_app._simpleListRegex);
                for (var j = 0; j < factories.length; j++) {
                    var editorFactory = factories[j];
                    var factoryName = editorFactory.replace(/\W/g, '$');
                    var factoryInstance = _app.EditorFactories[factoryName];
                    if (factoryInstance == null) {
                        try {
                            factoryInstance = eval(String.format('typeof {0}!="undefined"?new {0}():""', factoryName));
                        }
                        catch (ex) {
                            factoryInstance = '';
                        }
                        _app.EditorFactories[factoryName] = factoryInstance;
                    }
                    if (element == null)
                        return factoryInstance != '';
                    if (factoryInstance != '') {
                        if (!attach && Array.indexOf(_app._customInputElements, element.id) < 0)
                            return;
                        var viewType = this.get_viewType();
                        var result = attach ? factoryInstance.attach(element, viewType) : factoryInstance.detach(element, viewType);
                        if (result) {
                            if (attach)
                                Array.add(_app._customInputElements, element.id);
                            else
                                Array.remove(_app._customInputElements, element.id);
                            return true;
                        }
                    }
                }
            }
            return false;
        },
        _attachFieldBehaviors: function () {
            var that = this;
            if (that.editing()) {
                for (var i = 0; i < this.get_fields().length; i++) {
                    var field = this.get_fields()[i];
                    var element = this._get('_Item', field.Index); // $get(this.get_id() + '_Item' + field.Index);
                    var c = null;
                    if (element && !field.ReadOnly && !this._executeClientEditorFactories(field, element, true)) {
                        if (element.tagName == 'TEXTAREA' && field.Len > 0) {
                            element.setAttribute('maxlength', field.Len);
                            $addHandler(element, 'keyup', this._checkMaxLengthHandler)
                            $addHandler(element, 'blur', this._checkMaxLengthHandler)
                        }
                        if (!isNullOrEmpty(field.Mask)) {
                            var cc = Sys.CultureInfo.CurrentCulture;
                            var sdp = cc.dateTimeFormat.ShortDatePattern.toUpperCase().split(cc.dateTimeFormat.DateSeparator);
                            var m = $create(AjaxControlToolkit.MaskedEditBehavior, {
                                'CultureAMPMPlaceholder': cc.dateTimeFormat.AMDesignator + ';' + cc.dateTimeFormat.PMDesignator,
                                'CultureCurrencySymbolPlaceholder': cc.numberFormat.CurrencySymbol,
                                'CultureDateFormat': sdp[0].substring(0, 1) + sdp[1].substring(0, 1) + sdp[2].substring(0, 1),
                                'CultureDatePlaceholder': cc.dateTimeFormat.DateSeparator,
                                'CultureDecimalPlaceholder': cc.numberFormat.NumberDecimalSeparator,
                                'CultureName': cc.name,
                                'CultureThousandsPlaceholder': cc.numberFormat.NumberGroupSeparator,
                                'CultureTimePlaceholder': cc.dateTimeFormat.TimeSeparator,
                                'DisplayMoney': field.DataFormatString == '{0:c}',
                                'Mask': field.Mask,
                                'MaskType': field.MaskType,
                                'ClearMaskOnLostFocus': field.MaskType > 0,
                                'id': this.get_id() + '_MaskedEdit' + field.Index
                            },
                                null, null, element);
                            if (field.MaskType == 2) m.set_InputDirection(1);
                            Array.add(field.Behaviors, m);

                        }
                        if (field.Type.startsWith('Date')) {
                            c = $create(AjaxControlToolkit.CalendarBehavior, { id: this._get('_Calendar', field.Index)/* this.get_id() + '_Calendar' + field.Index*/ }, null, null, element);
                            c.set_format((field.DateFmtStr ? field.DateFmtStr : field.DataFormatString).match(/\{0:([\s\S]*?)\}/)[1]);
                            var button = $get(element.id + '_Button');
                            if (button) c.set_button(button);
                            if (field.TimeFmtStr) {
                                element = this._get('_Item$Time', field.Index);
                                if (element) {
                                    Array.add(field.Behaviors, c);
                                    c = this._attachTimeOptions(field, element);
                                }
                                //                            if (element) {
                                //                                Array.add(field.Behaviors, c);
                                //                                c = $create(Web.AutoComplete, {
                                //                                    'completionInterval': 500,
                                //                                    'contextKey': '', //String.format('{0},{1},{2}', this.get_controller(), this.get_viewId(), field.Name),
                                //                                    'delimiterCharacters': '',
                                //                                    'id': this.get_id() + '_AutoComplete$Time' + field.Index,
                                //                                    'minimumPrefixLength': field.AutoCompletePrefixLength,
                                //                                    //'serviceMethod': 'GetCompletionList',
                                //                                    //'servicePath': this.get_servicePath(),
                                //                                    //'useContextKey': true,
                                //                                    'typeCssClass': 'AutoComplete'
                                //                                },
                                //                                    null, null, element);
                                //                                Sys.UI.DomElement.addCssClass(c._completionListElement, 'Time');
                                //                                var cache = _app._timeOptions;
                                //                                if (!cache) {
                                //                                    cache = [];
                                //                                    var dt = new Date();
                                //                                    dt.setHours(dt.getHours(), 0, 0, 0);
                                //                                    while (cache.length < 24 * 2) {
                                //                                        Array.add(cache, String.localeFormat('{0:' + Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern + '}', dt));
                                //                                        dt.setMinutes(dt.getMinutes() + 30);
                                //                                    }
                                //                                    _app._timeOptions = cache;
                                //                                }
                                //                                (c._cache = [])['%'] = cache;
                                //                            }
                            }
                        }
                        else if (field.Type == 'TimeSpan')
                            c = this._attachTimeOptions(field, element);
                        else if (element.type == 'text' && field.AutoCompletePrefixLength > 0) {
                            c = $create(Web.AutoComplete, {
                                'completionInterval': 500,
                                'contextKey': String.format('{0},{1},{2}', this.get_controller(), this.get_viewId(), field.Name),
                                'delimiterCharacters': ',;',
                                'id': this.get_id() + '_AutoComplete' + field.Index,
                                'minimumPrefixLength': field.AutoCompletePrefixLength,
                                'serviceMethod': 'GetCompletionList',
                                'servicePath': this.get_servicePath(),
                                'useContextKey': true,
                                'typeCssClass': 'AutoComplete'
                            },
                                null, null, element);
                        }
                        else if (field.ItemsStyle == 'AutoComplete') {
                            var aliasField = this._allFields[field.AliasIndex];
                            element = this._get('_Item', aliasField.Index);
                            if (element && element.type == 'text')
                                c = $create(Web.AutoComplete, {
                                    'completionInterval': 500,
                                    'contextKey': String.format('Field:{0},{1}', this.get_id(), aliasField.Name),
                                    'delimiterCharacters': '',
                                    'id': this.get_id() + '_AutoComplete' + field.Index,
                                    'minimumPrefixLength': field.AutoCompletePrefixLength == 0 ? 1 : field.AutoCompletePrefixLength,
                                    'serviceMethod': 'GetPage',
                                    'servicePath': this.get_servicePath(),
                                    'useContextKey': true,
                                    'fieldName': field.Name,
                                    'enableCaching': false,
                                    'typeCssClass': 'Lookup'
                                },
                                    null, null, element);
                        }
                        if (c)
                            Array.add(field.Behaviors, c);
                    }
                    else if (field.OnDemand) {
                        var dropBox = $(this._container).find('.drop-box-' + field.Index);
                        if (dropBox.length)
                            if (field._dropBox) {
                                field._dropBoxInput.insertAfter(dropBox);
                                field._dropBox.insertAfter(dropBox);
                                dropBox.remove();
                                field._dropBoxInput = null
                                field._dropBox = null;
                            }
                            else
                                _app.upload('create', {
                                    container: dropBox,
                                    dataViewId: that._id,
                                    fieldName: field.Name
                                });
                    }
                }
            }
        },
        _detachBehaviors: function () {
            // detach quick find
            if (_touch) return;
            var e = this.get_quickFindElement();
            if (e) $clearHandlers(e);
            // detach row header drop downs and field behaviors
            //var editing = this.editing();
            if (this.get_fields() != null) {
                for (i = 0; i < this.get_fields().length; i++) {
                    var field = this.get_fields()[i];
                    var element = this._get('_Item', field.Index);
                    if (element && this._executeClientEditorFactories(field, element, false))
                        continue;
                    if (field.Len > 0 && field.Rows > 1) {
                        if (element && element.tagName == 'TEXTAREA' && element._events) {
                            $removeHandler(element, 'keyup', this._checkMaxLengthHandler);
                            $removeHandler(element, 'blur', this._checkMaxLengthHandler);
                        }
                    }
                    if (field._lookupModalBehavior != null) {
                        field._lookupModalBehavior.dispose();
                        field._lookupModalPanel.parentNode.removeChild(field._lookupModalPanel);
                        delete field._lookupModalPanel;
                        field._lookupModalBehavior = null;
                    }
                    if (field._lookupDataControllerBehavior != null) {
                        field._lookupDataControllerBehavior.dispose();
                        field._lookupDataControllerBehavior = null;
                    }
                    for (var j = 0; j < field.Behaviors.length; j++)
                        field.Behaviors[j].dispose();
                    Array.clear(field.Behaviors);
                    if (field.Editor)
                        _app.Editors[field.EditorId] = null;
                    if (field.OnDemand) {
                        var dropBox = $(this._container).find('.drop-box-' + field.Index);
                        if (dropBox.length && dropBox.is('.app-drop-box') && !dropBox.is('.app-drop-box-destroyed')) {
                            field._dropBoxInput = dropBox.next().detach();
                            field._dropBox = dropBox.detach();
                        }
                    }
                }
            }
        },
        _registerDataTypeFilterItems: function (family, parentItem, filterDef, activeFunc, field) {
            var item = null;
            for (var i = 0; i < filterDef.length; i++) {
                var fd = filterDef[i];
                if (!fd) {
                    if (!field.SearchOptions) {
                        item = new Web.Item(family);
                        parentItem.addChild(item);
                    }
                }
                else if (!fd.Hidden && (!field.SearchOptions || Array.indexOf(field.SearchOptions, fd.Function) >= 0)) {
                    item = new Web.Item(family, fd.Prompt ? fd.Text + '...' : fd.Text);
                    parentItem.addChild(item);
                    if (fd.List)
                        this._registerDataTypeFilterItems(family, item, fd.List, activeFunc, field);
                    else
                        if (fd.Prompt)
                            item.set_script('$find("{0}").showFieldFilter({1},"{2}","{3}")', this.get_id(), field.Index, fd.Function, fd.Text);
                        else
                            item.set_script('$find("{0}").applyFieldFilter({1},"{2}")', this.get_id(), field.Index, fd.Function);
                    if (activeFunc && fd.Function == activeFunc) {
                        var currItem = item;
                        while (currItem) {
                            currItem.set_selected(true);
                            currItem = currItem.get_parent();
                        }
                    }
                }
            }
        },
        _registerFieldHeaderItems: function (fieldIndex, containerFamily, containerItems, ignoreAggregates) {
            var startIndex = fieldIndex == null ? 0 : fieldIndex;
            var endIndex = fieldIndex == null ? this.get_fields().length - 1 : fieldIndex;
            var sort = this.get_sortExpression();
            if (sort) sort = sort.match(/^(\w+)\s+(asc|desc)/);
            for (var i = startIndex; i <= endIndex; i++) {
                var fieldFilter = null;
                var items = new Array();
                var family = containerFamily ? containerFamily : String.format('{0}$FieldHeaderSelector${1}', this.get_id(), i);
                var originalField = this.get_fields()[i];
                var field = this._allFields[originalField.AliasIndex];
                if (ignoreAggregates && field.Aggregate != 0) continue;
                if (field.AllowSorting || field.AllowQBE) {
                    var ascending = resourcesHeaderFilter.GenericSortAscending;
                    var descending = resourcesHeaderFilter.GenericSortDescending;
                    switch (field.FilterType) {
                        case 'Text':
                            ascending = resourcesHeaderFilter.StringSortAscending;
                            descending = resourcesHeaderFilter.StringSortDescending;
                            break;
                        case 'Date':
                            ascending = resourcesHeaderFilter.DateSortAscending;
                            descending = resourcesHeaderFilter.DateSortDescending;
                            break;
                    }
                    var allowSorting = field.AllowSorting && !this._sortingDisabled;
                    if (allowSorting) {
                        var sortedBy = sort && sort[1] == field.Name;
                        var item = new Web.Item(family, ascending);
                        if (sortedBy && sort[2] == 'asc')
                            item.set_selected(true);
                        else
                            item.set_cssClass('SortAscending');
                        item.set_script('$find("{0}").sort("{1} asc")', this.get_id(), field.Name);
                        Array.add(items, item);
                        item = new Web.Item(family, descending);
                        if (sortedBy && sort[2] == 'desc')
                            item.set_selected(true);
                        else
                            item.set_cssClass('SortDescending');
                        item.set_script('$find("{0}").sort("{1} desc")', this.get_id(), field.Name);
                        Array.add(items, item);
                    }
                    if (field.AllowQBE) {
                        fieldFilter = this.filterOf(field);
                        if (allowSorting) Array.add(items, new Web.Item())
                        item = new Web.Item(family, String.format(resourcesHeaderFilter.ClearFilter, field.HeaderText));
                        item.set_cssClass('FilterOff');
                        if (!fieldFilter) item.set_disabled(true);
                        item.set_script('$find("{0}").applyFilterByIndex({1},-1)', this.get_id(), originalField.AliasIndex);
                        Array.add(items, item);
                        var activeFunc = null;
                        if (!__designer()) {
                            var filterDef = resourcesDataFilters[field.FilterType];
                            if (field.Items.length == 0) {
                                item = new Web.Item(family, filterDef.Text);
                                Array.add(items, item);
                                activeFunc = this.get_fieldFilter(field, true);
                                this._registerDataTypeFilterItems(family, item, filterDef.List, activeFunc, field);
                            }
                            if (field.FilterType != 'Boolean' && (!field.SearchOptions || Array.indexOf(field.SearchOptions, '$in') >= 0) && field.AllowMultipleValues != false) {
                                item = new Web.Item(family, resourcesHeaderFilter.CustomFilterOption);
                                if (fieldFilter && fieldFilter.match(/\$(in|out)\$/))
                                    item.set_selected(true);
                                else
                                    item.set_cssClass('CustomFilter');
                                item.set_script('$find("{0}").showCustomFilter({1})', this.get_id(), originalField.AliasIndex);
                                Array.add(items, item);
                            }
                        }
                        if (originalField.AllowSamples != false)
                            if (originalField._listOfValues) {
                                if (fieldFilter && fieldFilter.startsWith('=')) {
                                    fieldFilter = fieldFilter.substring(1);
                                    if (fieldFilter.endsWith('\0'))
                                        fieldFilter = fieldFilter.substring(0, fieldFilter.length - 1);
                                }
                                for (var j = 0; j < originalField._listOfValues.length; j++) {
                                    if (j == 0) Array.add(items, new Web.Item());
                                    var v = originalField._listOfValues[j];
                                    var isSelected = false;
                                    var text = v;
                                    if (v == null)
                                        text = resourcesHeaderFilter.EmptyValue;
                                    else if (field.Items.length > 0) {
                                        item = this._findItemByValue(field, v);
                                        text = item[1];
                                    }
                                    else {
                                        if (field.Type == 'String' && v.length == 0)
                                            text = resourcesHeaderFilter.BlankValue;
                                        else if (!isNullOrEmpty(field.DataFormatString))
                                            text = field.Type.startsWith('DateTime') ? String.localeFormat('{0:d}', v) : field.format(v);
                                    }
                                    v = this.convertFieldValueToString(field, v); // == null ? 'null' : this.convertFieldValueToString(field, v);
                                    isSelected = activeFunc == '=' && fieldFilter && v == fieldFilter;
                                    if (text.length > resourcesHeaderFilter.MaxSampleTextLen) text = text.substring(0, resourcesHeaderFilter.MaxSampleTextLen) + '...';
                                    if (typeof (text) != 'string') text = text.toString();
                                    item = new Web.Item(family, text);
                                    if (isSelected) item.set_selected(true);
                                    item.set_script("$find(\'{0}\').applyFilterByIndex({1},{2});", this.get_id(), originalField.AliasIndex, j);
                                    item.set_group(1);
                                    Array.add(items, item);
                                }
                            }
                            else if (!field.SearchOptions || Array.indexOf(field.SearchOptions, '=') >= 0) {
                                item = new Web.Item(family, resourcesHeaderFilter.Loading);
                                item.set_dynamic(true);
                                item.set_script('$find("{0}")._loadListOfValues("{1}","{2}","{3}")', this.get_id(), family, originalField.Name, field.Name);
                                Array.add(items, item);
                            }
                    }
                }
                if (containerFamily) {
                    item = new Web.Item(containerFamily, field.HeaderText);
                    Array.add(containerItems, item);
                    for (j = 0; j < items.length; j++) {
                        var child = items[j];
                        item.addChild(child);
                    }
                    if (child.get_selected())
                        item.set_selected(true);
                    if (fieldFilter)
                        item.set_selected(true);
                }
                else
                    $registerItems(family, items, Web.HoverStyle.Click, Web.PopupPosition.Right, Web.ItemDescriptionStyle.ToolTip);
            }
        },
        _get_specialAction: function (type) {
            if (!this._specialActions) return null;
            var text = this._specialActions[type + 'Text'];
            var script = this._specialActions[type + 'Script'];
            return isNullOrEmpty(script) ? null : { 'text': text, 'script': script };
        },
        _registerSpecialAction: function (a, groupIndex, actionIndex, available) {
            if (!this._specialActions.PrintText && a.CommandName.match(/^Report/)) {
                if (available == null && !this._isActionAvailable(a)) return;
                this._specialActions.PrintText = a.HeaderText;
                this._specialActions.PrintScript = String.format('$find(\'{0}\').executeAction(\'ActionBar\',{1},-1,{2})', this.get_id(), actionIndex, groupIndex);
            }
            else if (a.CssClass == 'AttachIcon') {
                if (available == null && !this._isActionAvailable(a)) return;
                this._specialActions.AnnotateText = a.HeaderText;
                this._specialActions.AnnotateScript = String.format('$find(\'{0}\').executeAction(\'ActionBar\',{1},0,{2})', this.get_id(), actionIndex, groupIndex);
            }
        },
        _registerActionBarItems: function () {
            var groups = this.get_actionGroups('ActionBar');
            var isChart = this.get_isChart();
            if (isChart && !this.get_showViewSelector()) {
                var family = String.format('{0}${1}$ActionGroup$Chart', this.get_id(), this.get_viewId());
                var items = [];
                this._registerFieldHeaderItems(null, family, items, true);
                $registerItems(family, items, Web.HoverStyle.ClickAndStay, Web.PopupPosition.Left, Web.ItemDescriptionStyle.None);
            }
            this._specialActions = { 'PrintText': null, 'PrintScript': null, 'AnnotateText': null, 'AnnotateScript': null };
            for (var i = 0; i < groups.length; i++) {
                var group = groups[i];
                family = String.format('{0}${1}$ActionGroup${2}', this.get_id(), this.get_viewId(), i);
                if (!group.Flat) {
                    items = new Array();
                    for (var j = 0; j < group.Actions.length; j++) {
                        var a = group.Actions[j];
                        if (this._isActionAvailable(a)) {
                            var item = new Web.Item(family, a.HeaderText, a.Description);
                            item.set_cssClass(a.CssClassEx + (isNullOrEmpty(a.CssClass) ? a.CommandName + 'LargeIcon' : a.CssClass));
                            Array.add(items, item);
                            item.set_script(String.format('$find(\'{0}\').executeAction(\'ActionBar\',{1},null,{2})', this.get_id(), j, i));
                            this._registerSpecialAction(a, i, j, true);
                        }
                    }
                    $registerItems(family, items, Web.HoverStyle.ClickAndStay, Web.PopupPosition.Left, Web.ItemDescriptionStyle.Inline);
                }
                else {
                    $unregisterItems(family);
                    for (j = 0; j < group.Actions.length; j++)
                        this._registerSpecialAction(group.Actions[j], i, j);
                }
            }
            Array.clear(groups);
        },
        _registerViewSelectorItems: function () {
            var items = new Array(),
                that = this,
                family = that.get_id() + '$ViewSelector';
            for (var i = 0; i < that.get_views().length; i++) {
                var view = that.get_views()[i];
                if (view.Type != 'Form' && view.ShowInSelector || view.Id == that.get_viewId()) {
                    var item = new Web.Item(family, view.Label);
                    if (view.Id == this.get_viewId())
                        item.set_selected(true);
                    //item.set_script('$find("{0}").executeCommand({{commandName:"Select",commandArgument:"{1}"}})', this.get_id(), view.Id);
                    item.set_script(function (context) {
                        that.executeCommand({ commandName: 'Select', commandArgument: context.Id });
                    });
                    item.context(view);
                    Array.add(items, item);
                }
            }
            if (that.get_isChart()) {
                Array.add(items, new Web.Item());
                that._registerFieldHeaderItems(null, family, items, true);
            }
            $registerItems(family, items, Web.HoverStyle.ClickAndStay, Web.PopupPosition.Right, Web.ItemDescriptionStyle.None);
        },
        _registerRowSelectorItems: function () {
            var actions = this.get_actions('Grid');
            if (actions && actions.length > 0) {
                for (var i = 0; i < this._rows.length; i++) {
                    var items = new Array();
                    var family = String.format('{0}$RowSelector${1}', this.get_id(), i);
                    for (var j = 0; j < actions.length; j++) {
                        var a = actions[j];
                        if (this._isActionAvailable(a, i)) {
                            var item = new Web.Item(family, a.HeaderText, a.Description);
                            item.set_cssClass(a.CssClassEx + (isNullOrEmpty(a.CssClass) ? a.CommandName + 'Icon' : a.CssClass) + (items.length == 0 ? ' Default' : ''));
                            Array.add(items, item);
                            item.set_script(String.format('$find("{0}").executeAction("Grid", {1},{2})', this.get_id(), j, i));
                        }
                    }
                    $registerItems(family, items, Web.HoverStyle.Click, Web.PopupPosition.Right, Web.ItemDescriptionStyle.ToolTip);
                }
            }
            else
                for (i = 0; i < this._rows.length; i++)
                    $unregisterItems(String.format('{0}$RowSelector${1}', this.get_id(), i));
        },
        _get_searchBarSettings: function () {
            if (!this._searchBarSettings) this._searchBarSettings = [];
            var settings = this._searchBarSettings[this.get_viewId()];
            if (!settings) {
                var availableFields = [];
                var visibleFields = [];
                var currentFilter = this.get_filter();
                if (currentFilter.length > 0 && !this.filterIsExternal()) {
                    for (var i = 0; i < currentFilter.length; i++) {
                        var filter = currentFilter[i].match(_app._fieldFilterRegex);
                        var field = this.findField(filter[1]);
                        if (!field || this._fieldIsInExternalFilter(field)) continue;
                        var aliasField = this._allFields[field.AliasIndex];
                        var m = filter[2].match(_app._filterRegex);
                        if (!filter[2].startsWith('~')) {
                            field._renderedOnSearchBar = true;
                            var item = this._findItemByValue(field, m[3]);
                            var v = m[3]; // item == null ? m[3] : item[1];
                            if (v == 'null') v = '';
                            var vm = v.match(/^([\s\S]+?)\0?$/);
                            if (vm) v = vm[1];
                            var func = field.Items.length == 0 ? m[1] : '$in';
                            var fd = _app.filterDef(resourcesDataFilters[field.FilterType].List, field.FilterType == 'Boolean' && m[3].length > 1 ? (m[3] == '%js%true' ? '$true' : '$false') : func);
                            if (fd) {
                                var m2 = v.match(/^(.+?)\$and\$(.+?)$/);
                                Array.add(visibleFields, { 'Index': field.Index, 'Function': String.format('{0},{1}', fd.Function, fd.Prompt ? 'true' : 'false'), 'Value': m2 ? m2[1] : v, 'Value2': m2 ? m2[2] : '' });
                            }
                        }
                    }
                }
                var customSearch = false;
                for (i = 0; i < this._allFields.length; i++) {
                    field = this._allFields[i];
                    customSearch = field.Search == Web.FieldSearchMode.Required || field.Search == Web.FieldSearchMode.Suggested;
                    if (customSearch) break;
                }
                for (i = 0; i < this._allFields.length; i++) {
                    var originalField = this._allFields[i];
                    field = this._allFields[originalField.AliasIndex];
                    if (field.AllowQBE && field.Search != Web.FieldSearchMode.Forbidden && (!originalField.Hidden || originalField.Search != Web.FieldSearchMode.Default)) {
                        var visible = !customSearch && visibleFields.length < resources.Grid.VisibleSearchBarFields || (customSearch && (originalField.Search == Web.FieldSearchMode.Required || originalField.Search == Web.FieldSearchMode.Suggested));
                        if (!field._renderedOnSearchBar) {
                            var defaultFunction = field.Items.length == 0 || field.FilterType == 'Boolean' ? null : '$in';
                            if (field.SearchOptions)
                                defaultFunction = field.SearchOptions[0];
                            var dataFiltersList = resourcesDataFilters[field.FilterType].List;
                            for (var k = 0; k < dataFiltersList.length; k++) {
                                fd = dataFiltersList[k];
                                if (fd != null && (defaultFunction == null || fd.Function == defaultFunction))
                                    break;
                            }
                            var f = { 'Index': field.Index, 'Function': String.format('{0},{1}', fd.Function, fd.Prompt ? 'true' : 'false') };
                            if (visible || (this.findFieldUnderAlias(field) == field || field.VisibleOnSearchBar == null))
                                Array.add(visible ? visibleFields : availableFields, f);
                            field.VisibleOnSearchBar = visible;
                        }
                        field._renderedOnSearchBar = false;
                    }
                }
                settings = { 'visibleFields': visibleFields, 'availableFields': availableFields };
                this._searchBarSettings[this.get_viewId()] = settings;
            }
            return settings;
        },
        _lookupActionProcessing: function (enable) {
            if (enable != null)
                this._standardActionProcessing = !enable;
            return this._standardActionProcessing != true;
        },
        search: function (args) {
            var dataView = this,
                filter = [];
            if (args) {
                if (args.sortExpression) {
                    dataView._sortExpression = args.sortExpression;
                    var vitals = dataView.readContext('vitals');
                    if (vitals)
                        vitals.SortExpression = args.sortExpression;
                }
                if (args._filter)
                    dataView._filter = args._filter.slice(0);
                if (args.filter) {
                    function convertValue(v) {
                        if (Date.isInstanceOfType(v))
                            v = new Date(v - v.getTimezoneOffset() * 60 * 1000);
                        return String.format('%js%{0}', _serializer.serialize(v));
                    }
                    $(args.filter).each(function () {
                        var ff = this,
                            value = ff.value == null ? (ff.values) : ff.value,
                            op = ff.operator || '=',
                            sb;
                        // UnitPrice:$between$%js%10$and$%js%30
                        if (op.match(/\w/))
                            op = '$' + op + '$';
                        if (Array.isInstanceOfType(value)) {
                            sb = new Sys.StringBuilder();
                            $(value).each(function (index) {
                                if (index > 0)
                                    sb.append(op == '$between$' ? '$and$' : '$or$');
                                sb.append(convertValue(this));
                            });
                            value = sb.toString();
                        }
                        else
                            value = convertValue(value);
                        filter.push(String.format('{0}:{1}{2}', ff.name || ff.field, op, value));
                    });
                    dataView._filter = filter;
                }
                if (!args._init)
                    dataView.sync();
            }
            else {
                dataView._lookupActionProcessing(false);
                var result = dataView._hasSearchAction && dataView.executeActionInScope(['ActionBar', 'Form'], 'Search', null, -1);
                dataView._lookupActionProcessing(true);
                return result;
            }
        },
        _toggleSearchBar: function () {
            if (this._hasSearchShortcut && this.search())
                return;
            this.set_searchBarIsVisible(!this.get_searchBarIsVisible());
            this._updateSearchBar();
            if (this.get_searchBarIsVisible()) {
                if (this.get_lookupField())
                    this._adjustLookupSize();
                this._focusSearchBar();
            }
            _body_performResize();
        },
        _renderSearchBarFieldNameOptions: function (sb, field, settings) {
            if (field.Search == Web.FieldSearchMode.Required) return;
            for (var i = 0; i < settings.availableFields.length; i++) {
                var fieldInfo = settings.availableFields[i];
                var f = this._allFields[fieldInfo.Index];
                sb.appendFormat('<option value="{0}">{1}</option>', i, _app.htmlEncode(f.HeaderText));
            }
            //if (settings.visibleFields.length > 1)
            //    sb.appendFormat('<option value="{0}" class="Delete">{0}</option>', Web.DataViewResources.Grid.DeleteSearchBarField);
        },
        _renderSearchBarFunctionOptions: function (sb, field, filterDefs, fieldInfo, allowedFunction) {
            for (var i = 0; i < filterDefs.length; i++) {
                var fd = filterDefs[i];
                if (fd) {
                    if (fd.List)
                        this._renderSearchBarFunctionOptions(sb, field, fd.List, fieldInfo);
                    else if (allowedFunction == null || fd.Function == allowedFunction) {
                        var v = String.format('{0},{1}', _app.htmlAttributeEncode(fd.Function), fd.Prompt ? 'true' : 'false')
                        var selected = v == fieldInfo.Function ? ' selected="selected"' : '';
                        var option = String.format('<option value="{0}"{1}>{2}{3}</option>', v, selected, (fd.Prompt ? '' : resourcesDataFiltersLabels.Equals + ' '), !fd.Function.match(_app._keepCapitalization) ? fd.Text.toLowerCase() : fd.Text);
                        if (field.SearchOptions) {
                            var functionIndex = Array.indexOf(field.SearchOptions, fd.Function);
                            if (functionIndex >= 0)
                                field.SearchOptionSet[functionIndex] = option;
                        }
                        else
                            sb.append(option);
                    }
                }
            }
        },
        _renderSearchBarField: function (sb, settings, visibleIndex) {
            var fieldInfo = settings.visibleFields[visibleIndex];
            var field = this._allFields[fieldInfo.Index];
            var allowedFunction = field.Items.length == 0 || field.FilterType == 'Boolean' ? null : '$in';
            var funcInfo = fieldInfo.Function.match(/^(.+?),(true|false)$/);
            sb.appendFormat('<tr id="{0}$SearchBarField${1}"><td class="Control"><select id="{0}$SearchBarName${1}" tabindex="{3}" onchange="$find(\'{0}\')._searchBarNameChanged({1})"><option value="{1}" selected="selected">{2}</option>', this.get_id(), visibleIndex, _app.htmlEncode(field.HeaderText), $nextTabIndex());
            this._renderSearchBarFieldNameOptions(sb, field, settings);
            sb.append('</select></td>');
            sb.appendFormat('<td class="Control"><select id="{0}$SearchBarFunction${1}" class="Function" tabindex="{2}" onchange="$find(\'{0}\')._searchBarFuncChanged({1})">', this.get_id(), visibleIndex, $nextTabIndex());
            if (field.SearchOptions)
                field.SearchOptionSet = [];
            this._renderSearchBarFunctionOptions(sb, field, resourcesDataFilters[field.FilterType].List, fieldInfo, allowedFunction);
            if (field.SearchOptions)
                for (var i = 0; i < field.SearchOptionSet.length; i++)
                    sb.append(field.SearchOptionSet[i]);
            sb.append('</select></td>');
            var button = field.Type.startsWith('DateTime') ? '<a class="Calendar" href="javascript:" onclick="return false">&nbsp;</a>' : '';
            var isFilter = funcInfo[1] == '$in' || funcInfo[1] == '$notin';
            var fieldValue = fieldInfo.Value;
            if (isFilter) {
                var fsb = new Sys.StringBuilder();
                var hasValues = !isNullOrEmpty(fieldValue)
                fsb.appendFormat('<table class="FilterValues{3}" cellpadding="0" cellspacing="0" onmouseover="$app.highlightFilterValues(this,true,\'Active\')" onmouseout="$app.highlightFilterValues(this,false,\'Active\')"><tr><td class="Values" valign="top"><div><a class="Link" onclick="$find(\'{0}\')._showSearchBarFilter({1},{2});return false;" tabindex="{4}" href="javascript:" onfocus="$app.highlightFilterValues(this,true,\'Focused\')" onblur="$app.highlightFilterValues(this,false,\'Focused\')" title="{5}">', this.get_id(), field.Index, visibleIndex, hasValues ? '' : ' Empty', $nextTabIndex(), resourcesDataFiltersLabels.FilterToolTip);
                if (hasValues) {
                    var values = fieldValue.split(/\$or\$/);
                    for (i = 0; i < values.length; i++) {
                        if (i > 0)
                            fsb.append('<span class="Highlight">, </span>');
                        var v = values[i];
                        if (String.isJavaScriptNull(v))
                            v = resourcesHeaderFilter.EmptyValue;
                        else {
                            v = this.convertStringToFieldValue(field, v);
                            var item = this._findItemByValue(field, v);
                            v = item ? item[1] : field.format(v);
                        }
                        fsb.append(v);
                    }
                }
                else
                    fsb.append(resources.Lookup.SelectLink);
                fsb.appendFormat('</a></div></td><td class="Button{5}" valign="top"><a href="javascript:" onclick="$find(\'{0}\')._showSearchBarFilter({1},{2});return false" title="{3}" tabindex="{4}" onfocus="$app.highlightFilterValues(this,true,\'Focused\')" onblur="$app.highlightFilterValues(this,false,\'Focused\')" >&nbsp;</a></td></tr></table>', this.get_id(), hasValues ? -1 : field.Index, visibleIndex, hasValues ? resourcesDataFiltersLabels.Clear : resourcesDataFiltersLabels.FilterToolTip, $nextTabIndex(), hasValues ? ' Clear' : '');
                button = fsb.toString();
            }
            else if (!isNullOrEmpty(fieldValue))
                fieldValue = fieldValue.split(/\$or\$/)[0];
            if (funcInfo[2] == 'true') {
                if (typeof (fieldValue) == 'string' && !isFilter)
                    fieldValue = field.format(this.convertStringToFieldValue(field, fieldValue));
                sb.appendFormat('<td class="Control"><input id="{0}$SearchBarValue${1}" type="{6}" class="{2} {7}" value="{3}" tabindex="{4}"/>{5}</td>', this.get_id(), visibleIndex, field.FilterType, _app.htmlAttributeEncode(fieldValue == 'null' ? resourcesHeaderFilter.EmptyValue : fieldValue), $nextTabIndex(), button, isFilter ? 'hidden' : 'text', field.AllowAutoComplete != false ? ' AutoComplete' : '');
            }
            else
                sb.append('<td>&nbsp;</td>');
            var tabIndex = $nextTabIndex();
            if (funcInfo[1] == '$between')
                tabIndex = $nextTabIndex();
            sb.appendFormat('<td class="FieldAction"><a href="javascript:" tabindex="{1}" title="{2}" class="Remove" onclick="$find(\'{0}\')._searchBarNameChanged({3}, true)"><span></span</a></td>',
                this.get_id(), tabIndex, resources.Grid.RemoveCondition, visibleIndex);
            sb.append('</tr>');
            if (field.FilterType != 'Text' && funcInfo[1] == '$between') {
                var fieldValue2 = fieldInfo.Value2;
                if (typeof (fieldValue2) == 'string')
                    fieldValue2 = field.format(this.convertStringToFieldValue(field, fieldValue2));
                sb.appendFormat('<tr><td colspan="2" class="Control AndLabel">{4}</td><td><input id="{0}$SearchBarValue2${1}" type="text" class="{2}" value="{3}" tabindex="{5}"/>{6}</td><td>&nbsp;</td></tr>', this.get_id(), visibleIndex, field.FilterType, _app.htmlAttributeEncode(fieldValue2), resourcesDataFiltersLabels.And, tabIndex - 1, button);
            }
        },
        _focusSearchBar: function (visibleIndex) {
            var indexSpecified = visibleIndex != null;
            if (!indexSpecified) visibleIndex = 0;
            var funcElem = this._get_searchBarControl('Function', visibleIndex);
            if (!funcElem) {
                visibleIndex = 0;
                funcElem = this._get_searchBarControl('Function', 0);
            }
            var valElem = this._get_searchBarControl('Value', visibleIndex);
            if (valElem) {
                if (valElem.type == 'hidden') {
                    var a = valElem.parentNode.getElementsByTagName('a')[0];
                    a.focus();
                    if (indexSpecified && this._searchBarVisibleIndex == null && isNullOrEmpty(valElem.value))
                        a.click();
                }
                else {
                    Sys.UI.DomElement.setFocus(valElem);
                    //valElem.focus();
                    //valElem.select();
                }
            }
            else if (funcElem)
                funcElem.focus();
        },
        _searchBarNameChanged: function (visibleIndex, remove) {
            this._saveSearchBarSettings();
            var settings = this._get_searchBarSettings();
            var fieldInfo = settings.visibleFields[visibleIndex];

            if (!remove) {
                var nameElem = this._get_searchBarControl('Name', visibleIndex);
                var availableIndex = parseInteger(nameElem.value);
                settings.visibleFields[visibleIndex] = settings.availableFields[availableIndex];
                Array.removeAt(settings.availableFields, availableIndex);
            }
            else
                Array.removeAt(settings.visibleFields, visibleIndex);

            Array.insert(settings.availableFields, 0, fieldInfo);
            this._renderSearchBarControls(true);
            this._focusSearchBar(visibleIndex);
        },
        _searchBarFuncChanged: function (visibleIndex) {
            this._saveSearchBarSettings();
            var settings = this._get_searchBarSettings();
            var fieldInfo = settings.visibleFields[visibleIndex];
            var funcElem = this._get_searchBarControl('Function', visibleIndex);
            fieldInfo.Function = funcElem.value;
            this._renderSearchBarControls(true);
            this._focusSearchBar(visibleIndex);
        },
        _searchBarAddField: function () {
            this._saveSearchBarSettings();
            var settings = this._get_searchBarSettings();
            Array.add(settings.visibleFields, settings.availableFields[0]);
            Array.removeAt(settings.availableFields, 0);
            this._renderSearchBarControls(true);
            this._focusSearchBar(settings.visibleFields.length - 1);
        },
        _createSearchBarFilter: function (silent) {
            var oldFilter = Array.clone(this._filter);
            var settings = this._get_searchBarSettings();
            var filter = [];
            var success = true;
            for (var i = 0; i < settings.visibleFields.length; i++) {
                var fieldInfo = settings.visibleFields[i];
                var field = this._allFields[fieldInfo.Index];
                this.removeFromFilter(field);
                var funcInfo = fieldInfo.Function.match(/^(.+?),(true|false)$/);
                var values = [];
                if (funcInfo[2] == 'true') {
                    var valElem = this._get_searchBarControl('Value', i);
                    var val2Elem = this._get_searchBarControl('Value2', i);
                    if (isBlank(valElem.value) && (!val2Elem || isBlank(val2Elem.value)) && field.Search != Web.FieldSearchMode.Required)
                        continue;
                    if (funcInfo[1] == '$in' || funcInfo[1] == '$notin') {
                        Array.add(filter, { 'Index': field.Index, 'Function': funcInfo[1], 'Values': [valElem.value] });
                        continue;
                    }
                    if (isBlank(valElem.value)) {
                        if (silent)
                            continue;
                        else {
                            alert(resourcesValidator.RequiredField);
                            Sys.UI.DomElement.setFocus(valElem);
                            //valElem.focus();
                            //valElem.select();
                            success = false;
                        }
                        break;
                    }
                    var v = { NewValue: valElem.value.trim() };
                    var error = this._validateFieldValueFormat(field, v);
                    if (error) {
                        if (silent)
                            continue;
                        else {
                            alert(error);
                            Sys.UI.DomElement.setFocus(valElem);
                            //valElem.focus();
                            //valElem.select();
                            success = false;
                            break;
                        }
                    }
                    else
                        Array.add(values, field.Type.startsWith('DateTime') ? valElem.value.trim() : v.NewValue);
                    if (funcInfo[1] == '$between') {
                        if (isBlank(val2Elem.value)) {
                            if (silent)
                                continue;
                            else {
                                alert(resourcesValidator.RequiredField);
                                Sys.UI.DomElement.setFocus(val2Elem);
                                //val2Elem.focus();
                                //val2Elem.select();
                                success = false;
                            }
                            break;
                        }
                        v = { NewValue: val2Elem.value.trim() };
                        error = this._validateFieldValueFormat(field, v);
                        if (error) {
                            if (silent)
                                continue;
                            else {
                                alert(error);
                                Sys.UI.DomElement.setFocus(val2Elem);
                                //val2Elem.focus();
                                //val2Elem.select();
                                success = false;
                                break;
                            }
                        }
                        else
                            Array.add(values, field.Type.startsWith('DateTime') ? val2Elem.value.trim() : v.NewValue);
                    }
                    Array.add(filter, { 'Index': field.Index, 'Function': funcInfo[1], 'Values': values });
                }
                else
                    Array.add(filter, { 'Index': field.Index, 'Function': funcInfo[1], 'Values': null });
            }
            if (!success)
                return null;
            for (i = 0; i < settings.availableFields.length; i++) {
                fieldInfo = settings.availableFields[i];
                field = this._allFields[fieldInfo.Index];
                this.removeFromFilter(field);
            }
            for (i = 0; i < filter.length; i++) {
                var f = filter[i];
                this.applyFieldFilter(f.Index, f.Function, f.Values, true);
            }
            var newFilter = this._filter;
            this._filter = oldFilter;
            return newFilter;
        },
        _executeSearch: function (path) {
            var searchAction = this.get_action(path);
            if (searchAction && (!searchAction.Confirmation || !searchAction.Confirmation.match(/_controller\s*=/))) {
                var list = Sys.Application.getComponents();
                for (var i = 0; i < list.length; i++) {
                    var dataView = list[i];
                    if (_app.isInstanceOfType(dataView) && dataView._hasSearchAction) {
                        var searchAction2 = dataView.get_action(dataView._hasSearchAction);
                        if (searchAction2 && searchAction2.Confirmation) {
                            var m = searchAction2.Confirmation.match(/_controller\s*=\s*(\w+)/);
                            if (m && m[1] == this._controller)
                                dataView.search();
                        }
                    }
                }
            }
            else {
                this._clearSelectedKey();
                this._cancelConfirmation();
                this.refreshAndResize();
            }
        },
        _performSearch: function () {
            if (this._isBusy) return;
            this._saveSearchBarSettings();
            var filter = this._createSearchBarFilter(false);
            if (filter) {
                this.set_filter(filter);
                //            this.set_pageIndex(-2);
                //            this._loadPage();
                this.refreshData();
                this._setFocusOnSearchBar = true;
            }
            this._forgetSelectedRow(true);
        },
        _resetSearchBar: function () {
            if (this._isBusy) return;
            this.clearFilter();
            this._searchBarSettings[this.get_viewId()] = null;
            this._renderSearchBarControls(true);
            this._focusSearchBar();
        },
        _get_searchBarControl: function (type, visibleIndex) {
            return $get(String.format('{0}$SearchBar{1}${2}', this.get_id(), type, visibleIndex));
        },
        _saveSearchBarSettings: function () {
            var settings = this._get_searchBarSettings();
            for (var i = 0; i < settings.visibleFields.length; i++) {
                var fieldInfo = settings.visibleFields[i];
                var funcElem = this._get_searchBarControl('Function', i);
                var valElem = this._get_searchBarControl('Value', i);
                var val2Elem = this._get_searchBarControl('Value2', i);
                fieldInfo.Function = funcElem.value;
                if (valElem) {
                    valElem.value == resourcesHeaderFilter.EmptyValue ? 'null' : valElem.value;
                    fieldInfo.Value = this._formatSearchField(valElem, fieldInfo.Index);
                }
                if (val2Elem)
                    fieldInfo.Value2 = this._formatSearchField(val2Elem, fieldInfo.Index);
            }
        },
        _formatSearchField: function (input, fieldIndex) {
            var field = this._allFields[fieldIndex];
            if (field.Type.startsWith('Date')) {
                var d = Date.tryParseFuzzyDate(input.value, field.DataFormatString);
                if (d != null)
                    input.value = field.format(d);
            }
            else if (field.Type != 'String') {
                var n = Number.tryParse(input.value, field.DataFormatString);
                if (n != null)
                    input.value = field.format(n);
            }
            return input.value;
        },
        _renderSearchBarControls: function (force) {
            if (this._searchBarInitialized && !force) return;
            var sbc = $get(this.get_id() + '$SearchBarContent');
            this._searchBarInitialized = true;
            var sb = new Sys.StringBuilder();
            sb.append('<table class="SearchBarFrame">');

            var settings = this._get_searchBarSettings();

            for (var i = 0; i < settings.visibleFields.length; i++)
                this._renderSearchBarField(sb, settings, i);
            if (settings.availableFields.length > 0)
                sb.appendFormat('<tr><td colspan="3" class="AddConditionText"><a href="javascript:" onclick="$find(\'{0}\')._searchBarAddField();return false;">{3}</a></td><td class="FieldAction"><a href="javascript:" class="Add" tabindex="{1}" title="{2}" onclick="$find(\'{0}\')._searchBarAddField();return false;"><span></span></a></td>', this.get_id(), $nextTabIndex(), resources.Grid.AddCondition, resources.Grid.AddConditionText);

            sb.appendFormat('<tr><td><div id="{0}$SearchBarNameStub" class="Stub"></div></td><td><div id="{0}$SearchBarFuncStub" class="Stub"></div></td><td></td></tr>', this.get_id());

            sb.append('</table>');
            sb.appendFormat('<div class="SearchButtons"><button onclick="$find(\'{0}\')._performSearch();return false" tabindex="{3}" class="Search">{1}</button><br/><button onclick="$find(\'{0}\')._resetSearchBar();return false" tabindex="{4}" class="Reset">{2}</button></div>',
                this.get_id(), resources.Grid.PerformAdvancedSearch, resourcesDataFiltersLabels.Clear, $nextTabIndex(), $nextTabIndex());
            //if (settings.availableFields.length > 0)
            //    sb.appendFormat('<div class="SearchBarSize"><a href="javascript:" onclick="$find(\'{0}\')._searchBarAddField();return false;" class="More" tabindex="{2}"><span title="{1}"></span></a></div>', this.get_id(), resources.Grid.AddSearchBarField, $nextTabIndex());

            sbc.innerHTML = sb.toString();
            sb.clear();
            var stub = $get(this.get_id() + '$SearchBarNameStub');
            stub.style.width = stub.offsetWidth + 'px';
            stub = $get(this.get_id() + '$SearchBarFuncStub');
            stub.style.width = stub.offsetWidth + 'px';
            var selectors = sbc.getElementsByTagName('select');
            for (i = 0; i < selectors.length; i++)
                selectors[i].style.width = '100%';
            if (!this._searchBarExtenders) this._searchBarExtenders = [];
            for (i = 0; i < settings.visibleFields.length; i++) {
                var fieldInfo = settings.visibleFields[i];
                var field = this._allFields[fieldInfo.Index];
                var valElem = this._get_searchBarControl('Value', i);
                if (valElem) {
                    if (fieldInfo.Function.match(/\$(in|notin),/) == null) {
                        var c = this._createFieldInputExtender('SearchBar', field, valElem, i);
                        if (c) Array.add(this._searchBarExtenders, c);
                    }
                    else {
                        var parentRow = valElem;
                        while (parentRow && parentRow.tagName != 'TR')
                            parentRow = parentRow.parentNode;
                        for (var j = 0; j < parentRow.childNodes.length; j++)
                            parentRow.childNodes[j].vAlign = 'top';
                    }
                }
                var val2Elem = this._get_searchBarControl('Value2', i);
                if (val2Elem) {
                    c = this._createFieldInputExtender('SearchBar', field, val2Elem, i + '$2');
                    if (c) Array.add(this._searchBarExtenders, c);
                }
            }
        },
        _updateSearchBar: function () {
            var searchBar = this._get('$SearchBar');
            if (!searchBar) return;
            var isVisible = this.get_searchBarIsVisible();
            var sba = this._get('$SearchBarActivator');
            if (sba)
                if (isVisible)
                    Sys.UI.DomElement.addCssClass(sba, 'Activated');
                else
                    Sys.UI.DomElement.removeCssClass(sba, 'Activated');
            Sys.UI.DomElement.setVisible(searchBar, isVisible);
            if (isVisible)
                this._renderSearchBarControls(searchBar);
            if (sba) {
                Sys.UI.DomElement.setVisible(this._get('$QuickFind'), !isVisible);
                var searchToggle = this._get('$SearchToggle');
                if (this._hasSearchAction && this._hasSearchShortcut)
                    searchToggle.title = resources.Grid.PerformAdvancedSearch;
                else
                    searchToggle.title = this.get_searchBarIsVisible() ? resources.Grid.HideAdvancedSearch : resources.Grid.ShowAdvancedSearch;
            }
            var infoRow = this._get('$InfoRow');
            if (infoRow) {
                if (isVisible)
                    Sys.UI.DomElement.addCssClass(infoRow, 'WithSearchBar');
                else
                    Sys.UI.DomElement.removeCssClass(infoRow, 'WithSearchBar');
            }
            if (this._setFocusOnSearchBar) {
                this._setFocusOnSearchBar = false;
                this._focusSearchBar();
            }
        },
        _renderSearchBarActivator: function (sb) {
            if (!this.get_showSearchBar() || (!this.get_showQuickFind() && this.get_searchOnStart())) return;
            sb.appendFormat('<td class="SearchBarActivator{1}" id="{0}$SearchBarActivator"><a href="javascript:" onclick="$find(\'{0}\')._toggleSearchBar();return false;" id="{0}$SearchToggle"><span></span></a></td>', this.get_id(), this._hasSearchAction && this._hasSearchShortcut ? ' Search' : '');
            if (!this.get_showQuickFind())
                sb.append('<td class="Divider"><div></div></td>');
        },
        _internalRenderActionBar: function (sb) {
            var groups = this.get_actionGroups('ActionBar');
            sb.append('<table cellpadding="0" cellspacing="0" class="Groups"><tr>');
            var isGrid = this.get_isGrid();
            if (isGrid/*view.Type == 'Grid'*/)
                this._renderSearchBarActivator(sb);
            if (isGrid/*view.Type == 'Grid'*/ && this.get_showQuickFind()) {
                var s = this.get_quickFindText();
                sb.appendFormat('<td class="QuickFind" title="{2}" id="{0}$QuickFind"><div class="QuickFind"><table cellpadding="0" cellspacing="0"><tr><td><input type="text" id="{0}_QuickFind" value="{1}" class="{3}" tabindex="{4}"/></td><td class="Button"><a href="#" onclick="$find(\'{0}\').quickFind();return false;"><span>&nbsp;</span></a></td></tr></table></div></td>',
                    this.get_id(), _app.htmlAttributeEncode(s), resources.Grid.QuickFindToolTip, s == resources.Grid.QuickFindText ? 'Empty' : 'NonEmpty', $nextTabIndex());
                sb.append('<td class="Divider"><div></div></td>');
                if (this.get_lookupField() && !isNullOrEmpty(this.get_lookupField().ItemsNewDataView)) {
                    sb.appendFormat('<td class="QuickCreateNew"><a href="javascript:" onclick="$find(\'{0}\').closeLookupAndCreateNew();return false;" class="CreateNew" title="{1}" tabindex="{2}><span class="Placeholder"></span></a></td>', this.get_id(), resources.Lookup.GenericNewToolTip, $nextTabIndex());
                }
            }
            else {
                if (groups.length == 0 || this.get_lookupField())
                    sb.append('<td class="Divider"><div style="visibility:hidden"></div></td>');
            }


            if (!this.get_lookupField()) {
                // create action bar items
                this._registerActionBarItems();
                // render action group
                var showChartGroup = this.get_isChart() && !this.get_showViewSelector();
                if (showChartGroup)
                    sb.appendFormat('<td class="Group Main" onmouseover="$showHover(this,&quot;{0}${1}$ActionGroup$Chart&quot;,&quot;ActionGroup&quot;)" onmouseout="$hideHover(this)" onclick="$toggleHover()"><span class="Outer"><a href="javascript:" onfocus="$showHover(this,&quot;{0}${1}$ActionGroup$Chart&quot;,&quot;ActionGroup&quot;,2)" onblur="$hideHover(this)" tabindex="{3}" onclick="$hoverOver(this, 2);return false;">{2}</a></span></td>',
                        this.get_id(), this.get_viewId(), _app.htmlEncode(this.get_view().Label), $nextTabIndex());
                for (var i = 0; i < groups.length; i++) {
                    if (i > 0 || showChartGroup)
                        sb.append('<td class="Divider"><div></div></td>');
                    var group = groups[i];
                    if (group.Flat) {
                        var firstIndex = -1;
                        var lastIndex = -1;
                        for (var j = 0; j < group.Actions.length; j++) {
                            var a = group.Actions[j];
                            a._isAvailable = this._isActionAvailable(a) && !isNullOrEmpty(a.HeaderText);
                            a._isFirst = false;
                            a._isLast = false;
                            if (a._isAvailable) {
                                if (firstIndex == -1)
                                    firstIndex = j;
                                lastIndex = j;
                            }
                        }
                        if (firstIndex >= 0) {
                            group.Actions[firstIndex]._isFirst = true;
                            group.Actions[lastIndex]._isLast = true;
                        }
                        for (j = 0; j < group.Actions.length; j++) {
                            a = group.Actions[j];
                            if (a._isAvailable)
                                sb.appendFormat(
                                    '<td class="{7}Group FlatGroup{6}{8}{9}" onmouseover="$showHover(this,&quot;{0}${2}$ActionGroup${1}&quot;,&quot;ActionGroup&quot;)" onmouseout="$hideHover(this)" onclick="if(this._skip)this._skip=null;else $find(\'{0}\').executeAction(\'ActionBar\',{1},null,{2})" title="{10}"><span class="Outer">{5}<a href="javascript:" tabindex="{4}" onclick="this.parentNode.parentNode._skip=true;$find(\'{0}\').executeAction(\'ActionBar\',{1},null,{2});return false;" onfocus="$showHover(this,&quot;{0}${2}$ActionGroup${1}&quot;,&quot;ActionGroup&quot;,2)" onblur="$hideHover(this)">{3}</a></span></td>',
                                    this.get_id(), j, i, _app.htmlEncode(a.HeaderText), $nextTabIndex(),
                                    !isNullOrEmpty(a.CssClass) ? String.format('<span class="FlatGroupIcon {0}">&nbsp;</span>', _app.cssToIcon(a.CssClass)) : '',
                                    !isNullOrEmpty(a.CssClass) ? ' FlatGroupWithIcon' : '',
                                    a.CssClassEx,
                                    a._isFirst ? ' First' : '',
                                    a._isLast ? ' Last' : '', a.Description);
                        }
                    }
                    else
                        sb.appendFormat('<td class="{5}Group" onmouseover="$showHover(this,&quot;{0}${1}$ActionGroup${2}&quot;,&quot;ActionGroup&quot;)" onmouseout="$hideHover(this)" onclick="$toggleHover()"><span class="Outer"><a href="javascript:" onfocus="$showHover(this,&quot;{0}${1}$ActionGroup${2}&quot;,&quot;ActionGroup&quot;,2)" onblur="$hideHover(this)" tabindex="{4}" onclick="$hoverOver(this, 2);return false;">{3}</a></span></td>',
                            this.get_id(), this.get_viewId(), i, group.HeaderText, $nextTabIndex(), group.CssClassEx);
                }
            }
            sb.append('</tr></table>');
        },
        _renderActionBar: function (sb) {
            if (!this.get_showActionBar()) {
                this._registerActionBarItems();
                return;
            }
            sb.appendFormat('<tr class="ActionRow"><td colspan="{0}"  class="ActionBar">', this._get_colSpan());
            sb.appendFormat('<table style="width:100%" cellpadding="0" cellspacing="0"><tr><td style="width:100%" id="{0}$ActionBar">', this.get_id());

            this._internalRenderActionBar(sb);

            this._registerViewSelectorItems();
            sb.append('</td><td class="ViewSelectorControl">');
            if (this.get_showViewSelector()) {
                sb.appendFormat('<table cellpadding="0" cellspacing="0"><tr><td class="ViewSelectorLabel">{0}:</td><td>', resources.ActionBar.View);
                sb.appendFormat('<span class="ViewSelector" onmouseover="$showHover(this,&quot;{0}$ViewSelector&quot;,&quot;ViewSelector&quot;)" onmouseout="$hideHover(this)" onclick="$toggleHover()"><span class="Outer"><span class="Inner"><a href="javascript:" class="Link" tabindex="{2}" onfocus="$showHover(this,&quot;{0}$ViewSelector&quot;,&quot;ViewSelector&quot;,3)" onblur="$hideHover(this)">{1}</a></span></span></span>',
                    this.get_id(), _app.htmlEncode(this.get_view().Label), $nextTabIndex());
                sb.append('</td></tr></table>');
            }
            sb.append('</td></tr></table>');
            sb.append('</td></tr>');
        },
        get_statusBar: function () {
            var that = this,
                statusBar = that._statusBar || that._statusBarAuto;
            if (!statusBar && that._isWizard && !that.tagged('status-bar-disabled'))
                statusBar = that._statusBarAuto = that._controller + '.' + that._viewId + '._wizard: 0\n' + resourcesHeaderFilter.Loading + '>\n';
            return statusBar;
        },
        _renderStatusBar: function (sb) {
            sb.appendFormat('<tr class="StatusBarRow" style="display:none"><td colspan="{1}" class="StatusBar" id="{0}$StatusBar"></td></tr>', this.get_id(), this._get_colSpan());
        },
        statusBar: function () {
            var statusBar = this.get_statusBar(),
                smb;
            if (!isNullOrEmpty(statusBar)) {
                var iterator = /((\w+)\.)?((\w+)\.)?(\w+):\s*(.+?)\s*\n\s*(((.+?)\s*>\s*)+)/g,
                    m = iterator.exec(statusBar);
                while (m) {
                    var v = this.fieldValue(m[5]);
                    if (v == null) v = 'null';
                    if ((m[6] == v || m[6] == '*') && (isNullOrEmpty(m[2]) || m[2] == this.get_controller()) && (isNullOrEmpty(m[3]) || m[4] == this.get_viewId())) {
                        iterator = /(\[)?\s*(.+?)\s*(\])?\s*>\s*/g;
                        var segmentList = [];
                        var m2 = iterator.exec(m[7]);
                        while (m2) {
                            Array.add(segmentList, { 'Text': m2[2], 'Current': m2[1] == '[' && m2[3] == ']' });
                            m2 = iterator.exec(m[7]);
                        }
                        if (segmentList.length > 0 && !(__tf != 4)) {
                            smb = new Sys.StringBuilder('<ul class="StatusBar">');
                            var past = false;
                            var future = true;
                            for (var i = 0; i < segmentList.length; i++)
                                if (segmentList[i].Current) {
                                    past = true;
                                    future = false;
                                    break;
                                }

                            for (i = 0; i < segmentList.length; i++) {
                                var segment = segmentList[i];
                                var age = segment.Current ? 'Current' : (past ? 'Past' : (future ? 'Future' : ''));

                                var nextSegment = i < segmentList.length - 1 ? segmentList[i + 1] : null;
                                var transition = '';
                                if (nextSegment) {
                                    if (nextSegment.Current)
                                        transition = age + 'ToCurrent';
                                    else if (segment.Current)
                                        if (nextSegment.Current)
                                            transition = age + 'ToCurrent';
                                        else
                                            transition = age + 'ToFuture';
                                }

                                if (segment.Current) {
                                    past = false;
                                    future = true;
                                }
                                smb.appendFormat('<li class="Segment {1}{2}{3} {4}"><span class="Outer"><span class="Inner"><span class="Self">{0}</span></span></span></li>', _app.htmlEncode(segment.Text), age, i == 0 ? ' First' : '', i == segmentList.length - 1 ? ' Last' : '', transition);
                            }
                            smb.append('</ul>');
                        }
                        break;
                    }
                    m = iterator.exec(statusBar);
                }
            }
            return smb ? smb.toString() : null;
        },
        _updateStatusBar: function () {
            var statusBar = this.get_statusBar(),
                html,
                statusBarCell;
            if (!isNullOrEmpty(statusBar)) {
                statusBarCell = $(this._get('$StatusBar'));
                if (statusBarCell.length) {
                    html = this.statusBar();
                    if (html)
                        statusBarCell.html(html).parent().show();
                    else
                        statusBarCell.parent().hide();
                }
            }
        },
        filterByFirstLetter: function (index) {
            var fieldName = this._firstLetters[0];
            var letter = this._firstLetters[index]
            this.applyFieldFilter(this.findField(fieldName).Index, "$beginswith", [letter]);
        },
        _renderViewDescription: function (sb) {
            var showLetters = this._firstLetters && this._firstLetters.length > 2;
            var showDescription = this.get_showDescription();
            if (!showDescription && !showLetters) return;
            var t = showDescription ? this.get_description() : '';
            var lt = null;
            if (showLetters) {
                var lsb = new Sys.StringBuilder();
                lsb.append('<div class="Letters">');
                var field = this.findField(this._firstLetters[0]);
                var selectedLetter = null;
                for (var i = 0; i < this._filter.length; i++) {
                    var m = this._filter[i].match(_app._fieldFilterRegex);
                    if (m && m[1] == field.Name) {
                        m = m[2].match(/^\$beginswith\$(.+?)\0$/);
                        if (m)
                            selectedLetter = this.convertStringToFieldValue(field, m[1]);
                        break;
                    }
                }
                var beginsWith = String.format('{0} {1} ', _app.htmlAttributeEncode(field.Label), resourcesDataFilters.Text.List[2].Text.toLowerCase());
                for (i = 1; i < this._firstLetters.length; i++) {
                    var letter = this._firstLetters[i];
                    lsb.appendFormat('<a href="javascript:" onclick="$find(\'{0}\').filterByFirstLetter({1});return false;" class="Letter{5}" title="{3}{4}">{2}</a> ',
                        this.get_id(), i, letter, beginsWith, _app.htmlAttributeEncode(letter), selectedLetter == letter ? ' Selected' : '');
                }
                lsb.append('</div>');
                lt = lsb.toString();
                lsb.clear();
            }
            if (showDescription && isNullOrEmpty(t))
                t = this.get_view().HeaderText;
            //var isTree = this.get_isTree();
            if (!isNullOrEmpty(t) || this.get_lookupField() || /*isTree || */lt) {
                sb.appendFormat('<tr class="HeaderTextRow"><td colspan="{0}" class="HeaderText">', this._get_colSpan());
                if (this.get_lookupField() != null)
                    sb.append('<table style="width:100%" cellpadding="0" cellspacing="0"><tr><td style="padding:0px">');
                sb.appendFormat('<div id="{0}$HeaderText">', this.get_id());
                sb.append(this._formatViewText(resources.Views.DefaultDescriptions[t], true, t));
                sb.append('</div>');
                if (lt)
                    sb.append(lt);
                if (this.get_lookupField() != null)
                    sb.appendFormat('</td><td align="right" style="padding:0px"><a href="javascript:" class="Close" onclick="$find(\'{0}\').hideLookup();return false" tabindex="{2}" title="{1}">&nbsp;</a></td></tr></table>', this.get_id(), resourcesModalPopup.Close, $nextTabIndex());
                /*if (isTree) {
                var path = this.get_path();
                sb.append('<div class="Path">');
                sb.appendFormat('<a href="javascript:" onclick="return false" class="Toggle" title="{1}"><span>&nbsp;</span></a>', this.get_id(), _app.htmlAttributeEncode(resources.Grid.FlatTreeToggle));
                for (i = 0; i < path.length; i++) {
                var levelInfo = path[i];
                sb.appendFormat('<span class="Divider"></span><a href="javascript:" class="Node{4}" onclick="$find(\'{0}\').drillIn({1})" title="{3}">{2}</a>', this.get_id(), i,
                _app.htmlEncode(levelInfo.text),
                _app.htmlAttributeEncode(String.format(resources.Lookup.SelectToolTip, '"' + levelInfo.text + '"')),
                i == path.length - 1 ? ' Selected' : '');
                }
                sb.append('</div>');
                }*/
                sb.append('</td></tr>');
            }
        },
        _renderInfoBar: function (sb) {
            var filter = this.get_filter();
            if (filter.length > 0 && !this.filterIsExternal()) {
                var fsb = new Sys.StringBuilder();
                if (this.get_viewType() != "Form")
                    this._renderFilterDetails(fsb, filter);
                if (this.get_viewType() != "Form" && !fsb.isEmpty()) {
                    sb.appendFormat('<tr class="InfoRow {2}" id="{0}$InfoRow"><td colspan="{1}">', this.get_id(), this._get_colSpan(), this.get_viewType());
                    sb.append(fsb.toString());
                    sb.append('</td></tr>');
                }
                fsb.clear();
            }
        },
        _renderFilterDetails: function (sb, currentFilter, includeBanner) {
            var bannerRendered = false,
                that = this,
                hint, firstDataView,
                deepSearchInfo = that.viewProp && that.viewProp('deepSearchInfo'),
                matchCount = 0,
                conditionCount,
                i;

            function compressMatchScope(scope) {
                var j = i + 1;
                while (j < currentFilter.length) {
                    if (currentFilter[j].match(/^(_match_|_donotmatch)/))
                        break;
                    j++;
                }
                if (j - i > 2)
                    return resourcesMobile[scope + 'PastTense']
                else
                    if (scope.startsWith('DoNot'))
                        return resourcesMobile.DidNotMatch;
                    else
                        return resourcesMobile.Matched;
            }

            function compressValue(value, text, isSecondValue) {
                // remove time if 12:00AM or 11:59:59PM
                if (value.getHours)
                    if ((value.getHours() == 0 && value.getMinutes() == 0 && value.getSeconds() == 0) || (isSecondValue && value.getHours() == 23 && value.getMinutes() == 59 && value.getSeconds() == 59))
                        return String.format('{0:d}', value);
                return text;
            }

            //var checkRecursive = true;
            for (i = 0; i < currentFilter.length; i++) {
                var filter = currentFilter[i].match(_app._fieldFilterRegex),
                    isQuickFind = filter[1] == '_quickfind_',
                    field = isQuickFind ? that._fields[0] : that.findField(filter[1]),
                    isDeepSearch,
                    scope;
                //var recursive = field && field.ItemsDataController == this.get_controller() && this.get_isTree() && checkRecursive;
                //if (recursive)
                //    checkRecursive = false;
                if (!field && filter[1].match(/\,/) && deepSearchInfo) {
                    if (conditionCount++ > 0)
                        sb.append('; ');
                    sb.append(deepSearchInfo[filter[1].replace(/\W/g, '_') + (matchCount - 1)]);
                    continue;
                }
                if (!field /*&& filter[1] != '_quickfind_'*/ || this._fieldIsInExternalFilter(field)/* || recursive*/) {
                    if (filter[1] == '_match_')
                        scope = 'Match';
                    else if (filter[1] == '_donotmatch_')
                        scope = 'DoNotMatch';
                    if (scope) {
                        if (filter[2] == '$all$')
                            scope += 'All';
                        else
                            scope += 'Any';
                        if (matchCount > 0)
                            sb.append('. ');
                        sb.append(compressMatchScope(scope));
                        sb.append(': ');
                        matchCount++;
                        conditionCount = 0;
                    }
                    continue;
                }
                if (!bannerRendered) {
                    if (includeBanner != false)
                        sb.appendFormat('<a href="javascript:" onclick="$find(\'{0}\').clearFilter(true);return false" class="Close" tabindex="{3}" title="{2}">&nbsp;</a><span class="Details"><span class="Information">&nbsp;</span>{1} ', this.get_id(), resources.InfoBar.FilterApplied, resourcesModalPopup.Close, $nextTabIndex());
                    bannerRendered = true;
                }
                var aliasField = field && this._allFields[field.AliasIndex],
                    m = _app._filterIteratorRegex.exec(filter[2]), //var m = filter[2].match(_app._filterRegex);
                    first = true,
                    compressDate = false;
                while (m && (m[1].startsWith('~') || !(field.Index == field.AliasIndex && field.IsPrimaryKey && field.Hidden))) {
                    if (!first)
                        sb.append(', ');
                    else
                        sb.appendFormat('<span class="FilterElement" onclick="$find(\'{0}\').removeFromFilterByIndex({1})" title="{2}">', this.get_id(), isQuickFind ? -1 : field.Index, isQuickFind ? resourcesDataFiltersLabels.Clear : _app.htmlAttributeEncode(String.format(resourcesHeaderFilter.ClearFilter, field.Label)));

                    if (m[1].startsWith('~')) {
                        sb.appendFormat(String.format('{0} <b class="String">{1}</b>', resources.InfoBar.QuickFind, this.convertStringToFieldValue(field, m[3])));
                        hint = _touch && that.viewProp('quickFindHint');
                        if (hint) {
                            hint = hint.split(';');
                            sb.append(' ' + resourcesMobile.In + ' ');

                            if (hint[0])
                                sb.append(that.get_view().Label);
                            else
                                firstDataView = true;
                            $(_touch._pages).each(function () {
                                var page = this,
                                    dv = page.dataView,
                                    hintIndex;
                                if (dv) {
                                    if (that._id == dv._filterSource) {
                                        hintIndex = hint.indexOf(dv._controller + '.' + dv._viewId + '.' + dv._filterFields);
                                        if (hintIndex != -1) {
                                            if (firstDataView)
                                                firstDataView = false;
                                            else if (hintIndex == hint.length - 1)
                                                sb.append(' ' + resourcesDataFiltersLabels.And + ' ');
                                            else
                                                sb.append(', ');
                                            sb.append(dv.get_view() && dv.get_view().Label || page.text);
                                        }
                                    }
                                }

                            });
                            //sb.append('.');
                        }
                    }
                    else {
                        if (conditionCount++ > 0)
                            sb.append('; ');
                        sb.appendFormat('<span class="Highlight">{0}</span>', that._fieldNameHint && that._fieldNameHint[aliasField.Name] || aliasField.HeaderText);
                        var fd = _app.filterDef(resourcesDataFilters[field.FilterType].List, field.FilterType == 'Boolean' && m[3].length > 1 ? (m[3] == '%js%true' ? '$true' : '$false') : m[1]);
                        if (!fd) {
                            switch (m[1]) {
                                case '=':
                                    sb.append(String.isJavaScriptNull(m[2]) ? resources.InfoBar.Empty : resources.InfoBar.EqualTo);
                                    break;
                                case '<':
                                    sb.append(resources.InfoBar.LessThan);
                                    break;
                                case '<=':
                                    sb.append(resources.InfoBar.LessThanOrEqual);
                                    break;
                                case '>':
                                    sb.append(resources.InfoBar.GreaterThan);
                                    break;
                                case '>=':
                                    sb.append(resources.InfoBar.GreaterThanOrEqual);
                                    break;
                                case '*':
                                    sb.append(m[2].startsWith('%') ? resources.InfoBar.Like : resources.InfoBar.StartsWith);
                                    break;
                            }
                            var item = this._findItemByValue(field, this.convertStringToFieldValue(field, m[3]));
                            var v = item == null ? m[3] : item[1];
                            if (String.isJavaScriptNull(m[3]) || isBlank(v))
                                v = resources.InfoBar.Empty;
                            else
                                v = this.convertStringToFieldValue(field, v);
                            sb.appendFormat('<b>{0}</b>', String.htmlEncode(v));
                        }
                        else if (fd.Prompt) {
                            sb.appendFormat(' {0} ', fd.Text.toLowerCase());
                            //item = m[1].match(/\$(in|notin|between)\$/) ? null : this._findItemByValue(field, this.convertStringToFieldValue(field, m[3]));
                            //v = m[3] : item[1];
                            v = m[3];
                            var values = v.split(_app._listRegex);
                            if (String.isJavaScriptNull(values[0])) values[0] = resources.InfoBar.Empty;
                            if (!String.isJavaScriptNull(m[2])) {
                                var vm = values[0].match(/^([\s\S]+?)\0?$/);
                                if (vm) values[0] = vm[1];
                                v = this.convertStringToFieldValue(field, values[0]);
                                item = this._findItemByValue(field, v);
                                sb.appendFormat('<b>{0}</b>', String.htmlEncode(item ? item[1] : compressValue(v, field.format(v))));
                                for (var j = 1; j < values.length; j++) {
                                    sb.appendFormat('{0} ', m[1] == '$between$' ? ' ' + resourcesDataFiltersLabels.And : ', ');
                                    v = this.convertStringToFieldValue(field, values[j]);
                                    if (v == null)
                                        v = resourcesHeaderFilter.EmptyValue;
                                    else {
                                        item = this._findItemByValue(field, v);
                                        v = item ? item[1] : compressValue(v, field.format(v), true);
                                    }
                                    sb.appendFormat('<b class="{1}">{0}</b>', String.htmlEncode(v));
                                    if (j > 5) {
                                        sb.append(', ..');
                                        break;
                                    }
                                }
                            }
                        }
                        else
                            sb.appendFormat(' {0} <b>{1}</b>', resourcesDataFiltersLabels.Equals, fd.Function.match(_app._keepCapitalization) ? fd.Text : fd.Text.toLowerCase());
                    }
                    m = _app._filterIteratorRegex.exec(filter[2]);
                    first = false;
                }
                if (!first && !matchCount && !that._fieldNameHint)
                    sb.append('.');
                sb.append('</span> ');
            }
            if (matchCount)
                sb.append('. ');
            sb.append('</span>');
        },
        _findItemByValue: function (field, value) {
            var lov = field.DynamicItems || field.Items,
                itemCache = field.ItemCache,
                result;
            if (lov.length == 0) return null;
            //value = value == null ? '' : value.toString();
            if (!itemCache) {
                //for (var i = 0; i < lov.length; i++) {
                //    var item = lov[i];
                //    var itemValue = item[0] == null ? "" : item[0].toString();
                //    if (itemValue == value)
                //        return item;
                //}
                itemCache = field.ItemCache = {};
                $(lov).each(function () {
                    var v = this;
                    itemCache[v[0]] = v;
                })
            }
            return field.ContextFields ? [value, value] : (itemCache[value] || [null, this.get_isForm() || _touch/* this.get_viewType() == 'Form'*/ ? resourcesData.NullValueInForms : resourcesData.NullValue]);
        },
        _renderPager: function (sb, location) {
            if (this.get_showPager().indexOf(location) == -1) return;
            var isChart = this.get_isChart();
            sb.appendFormat('<tr class="FooterRow {2}PagerRow"><td colspan="{0}" class="Footer"><table cellpadding="0" cellspacing="0" style="width:100%"><tr><td align="left" class="Pager PageButtons{1}">', this._get_colSpan(), isChart ? ' Print' : '', location);
            var pageCount = this.get_pageCount();
            var pageSize = this.get_pageSize();
            if (isChart) {
                pageCount = 1;
                pageSize = this._totalRowCount;
                var printAction = this._get_specialAction('Print');
                if (printAction)
                    sb.appendFormat('<a href="javascript:" onclick="{1};return false;" title="{0}" class="Print"><span></span></a></td><td>', printAction.text, printAction.script);
            }
            if (pageCount > 1) {
                var buttonIndex = this._firstPageButtonIndex;
                var pagerButtonCount = this.get_pagerButtonCount();
                var buttonCount = pagerButtonCount;
                if (this.get_pageIndex() > 0)
                    sb.appendFormat('<a href="#" onclick="$find(\'{1}\').goToPage({0},true);return false" class="PaddedLink" tabindex="{3}">{2}</a>', this.get_pageIndex() - 1, this.get_id(), resourcesPager.Previous, $nextTabIndex());
                else
                    sb.appendFormat('<span class="Disabled">{0}</span>', resourcesPager.Previous);
                sb.appendFormat(' | {0}: ', resourcesPager.Page);
                if (buttonIndex > 0)
                    sb.appendFormat('<a href="#" onclick="$find(\'{1}\').goToPage(0,true);return false" class="PaddedLink" tabindex="{2}">1</a><a href="#" onclick="$find(\'{1}\').goToPage({0},true);return false" class="PaddedLink" tabindex="{3}">...</a>', buttonIndex - 1, this.get_id(), $nextTabIndex(), $nextTabIndex());
                while (buttonCount > 0 && buttonIndex < pageCount) {
                    if (buttonIndex == this.get_pageIndex())
                        sb.appendFormat('<span class="Selected">{0}</span>', buttonIndex + 1);
                    else
                        sb.appendFormat('<a href="#" onclick="$find(\'{1}\').goToPage({0},true);return false" class="PaddedLink" tabindex="{3}">{2}</a>', buttonIndex, this.get_id(), buttonIndex + 1, $nextTabIndex());
                    buttonIndex++;
                    buttonCount--;
                }
                if (buttonIndex <= pageCount - 1)
                    sb.appendFormat('<a href="#" onclick="$find(\'{1}\').goToPage({0},true);return false" class="PaddedLink" tabindex="{2}">...</a><a href="#" onclick="$find(\'{1}\').goToPage({3}-1,true);return false" class="PaddedLink" tabindex="{4}">{3}</a>', this._firstPageButtonIndex + pagerButtonCount, this.get_id(), $nextTabIndex(), pageCount, $nextTabIndex());
                sb.append(' | ');
                if (this.get_pageIndex() < pageCount - 1)
                    sb.appendFormat('<a href="#" onclick="$find(\'{1}\').goToPage({0},true);return false" class="PaddedLink" tabindex="{3}">{2}</a>', this.get_pageIndex() + 1, this.get_id(), resourcesPager.Next, $nextTabIndex());
                else
                    sb.appendFormat('<span class="Disabled">{0}</span>', resourcesPager.Next);
            }
            sb.append('</td><td align="right" class="Pager PageSize">&nbsp;');
            var showPageSize = this.get_showPageSize();
            var pageSizes = this._pageSizes;
            if (showPageSize && this._totalRowCount > pageSize) {
                sb.append(resourcesPager.ItemsPerPage);
                for (i = 0; i < pageSizes.length; i++) {
                    if (i > 0) sb.append(', ');
                    if (pageSize == pageSizes[i])
                        sb.appendFormat('<b>{0}</b>', pageSize);
                    else
                        sb.appendFormat('<a href="#" onclick="$find(\'{0}\').set_pageSize({1},true);return false" tabindex="{2}">{1}</a>', this.get_id(), pageSizes[i], $nextTabIndex());
                }
                sb.append(' | ');
            }
            if (this._totalRowCount > 0) {
                var lastVisibleItemIndex = (this.get_pageIndex() + 1) * pageSize;
                if (lastVisibleItemIndex > this._totalRowCount) lastVisibleItemIndex = this._totalRowCount;
                if (showPageSize)
                    sb.appendFormat(resourcesPager.ShowingItems, this.get_pageIndex() * pageSize + 1, lastVisibleItemIndex, this._totalRowCount);
                var multipleSelection = this.multiSelect();
                if (multipleSelection) {
                    sb.appendFormat('<span id="{0}$SelectionInfo">', this.get_id());
                    if (this._selectedKeyList.length > 0) sb.appendFormat(resourcesPager.SelectionInfo, this._selectedKeyList.length);
                    sb.append('</span>');
                }
                if (showPageSize || multipleSelection)
                    sb.append(' | ');
            }
            sb.appendFormat('</td><td align="center" class="Pager Refresh" id="{0}_Wait">', this.get_id());
            if (!this.get_searchOnStart())
                sb.appendFormat('<a href="#" onclick="$find(\'{0}\').{3}();return false" tabindex="{2}" title="{1}"><span>&nbsp;</span></a>', this.get_id(), resourcesPager.Refresh, $nextTabIndex(), this._hasSearchAction ? 'search' : 'sync');
            sb.append('</td></tr></table>');
            sb.append('</td></tr>');
        },
        refreshAndResize: function () {
            this.cancelDataSheetEdit();
            this.goToPage(-1);
            delete this._viewColumnSettings;
        },
        refreshData: function () {
            var that = this;
            if (!that._busy()) {
                that._clearCache();
                that.set_pageIndex(-2);
                that._loadPage();
            }
        },
        _dittoCollectedValues: function (newValues, fieldToIgnore) {
            if (this.editing()) {
                var ignoreRegex = fieldToIgnore ? new RegExp(String.format('^{0}(Length|ContentType|FileName|FullFileName)?$', fieldToIgnore)) : null;
                var values = this._collectFieldValues(true);
                var ditto = [];
                for (var i = 0; i < values.length; i++) {
                    var v = values[i];
                    if (!ignoreRegex || !v.Name.match(ignoreRegex))
                        Array.add(ditto, { 'name': v.Name, 'value': v.Modified ? v.NewValue : v.OldValue });
                }
                if (newValues) {
                    for (i = 0; i < newValues.length; i++) {
                        v = newValues[i];
                        for (var j = 0; j < ditto.length; j++) {
                            var name = v.Name ? v.Name : v.name;
                            if (ditto[j].name == name) {
                                Array.removeAt(ditto, j);
                                break;
                            }
                        }
                        Array.add(ditto, v.Name ? { 'name': v.Name, 'value': v.NewValue } : v);
                    }
                }
                this._ditto = ditto;
            }
        },
        refresh: function (noFetch, newValues, fieldToIgnore) {
            var that = this;
            that._dittoCollectedValues(newValues, fieldToIgnore);
            that._lastSelectedCategoryTabIndex = that.get_categoryTabIndex();
            if (noFetch)
                that._render();
            else {
                if (_touch) {
                    var parentDataView = that.get_parentDataView(this);
                    if (parentDataView == that)
                        that.sync();
                    else
                        parentDataView._syncKey = parentDataView.get_selectedKey();
                }
                else {
                    that.goToView(that.get_viewId());
                    that._ditto = null;
                }
            }
        },
        //    _initContext: function () {
        //        if (this._context == null) {
        //            var c = $get('__COTSTATE'); // $get(this.get_id() + '$Context');//
        //            if (c && !isNullOrEmpty(c.value))
        //                this._context = Sys.Serialization.JavaScriptSerializer.deserialize(c.value);
        //            else
        //                this._context = {};
        //        }
        //    },
        //    _saveContext: function () {
        //        var c = $get('__COTSTATE'); // $get(this.get_id() + '$Context'); // 
        //        if (c)
        //            c.value = Sys.Serialization.JavaScriptSerializer.serialize(this._context);
        //    },
        _cname: function (name) {
            var viewId = this.get_viewId();
            if (viewId == null)
                viewId = 'grid1';
            return String.format('{0}${1}${2}', this._id, viewId, name);
        },
        readContext: function (name) {
            return Web.PageState.read(this._cname(name));
        },
        writeContext: function (name, value) {
            Web.PageState.write(this._cname(name), value);
        },
        _saveViewVitals: function () {
            this.writeContext('vitals', {
                'PageIndex': this.get_pageIndex(),
                'PageSize': this.get_pageSize(),
                'Filter': this.get_filter(),
                'SortExpression': this.get_sortExpression(),
                'Tag': this.get_tag()
            });
        },
        _restoreViewVitals: function (request) {
            if (request.PageIndex >= 0) return;
            var vitals = this.readContext('vitals');
            if (vitals == null) return;
            request.RequiresRowCount = true;
            request.RequiresMetaData = true;
            if (request.PageIndex == -1 && vitals.Filter && request.Filter) {
                request.Tag = vitals.Tag;
                request.PageIndex = vitals.PageIndex;
                request.PageSize = vitals.PageSize;
                if (request.FilterIsExternal) {
                    var newFilter = request.Filter;
                    for (var i = 0; i < vitals.Filter.length; i++) {
                        var lastFilterExpression = vitals.Filter[i];
                        var lastFilterExpressionFieldName = lastFilterExpression.substring(0, lastFilterExpression.indexOf(':'));
                        var found = false;
                        for (var j = 0; j < request.Filter.length; j++) {
                            var newFilterExpression = request.Filter[j];
                            var newExpressionFilterName = newFilterExpression.substring(0, newFilterExpression.indexOf(':'));
                            if (newExpressionFilterName == lastFilterExpressionFieldName) {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            Array.add(newFilter, lastFilterExpression);
                    }
                    request.Filter = Array.clone(newFilter);
                }
                else
                    if (request.Filter == null || request.Filter.length == 0)
                        request.Filter = Array.clone(vitals.Filter);
                request.SortExpression = vitals.SortExpression;
                if (!this.get_isDataSheet() || !this._get_focusedCell()) {
                    var gridType = this.readContext('GridType');
                    if (gridType != null)
                        this.changeViewType(gridType);
                }
            }
        },
        paramValue: function (name, value) {
            if (!this._paramValues)
                this._paramValues = [];
            this._paramValues.push({ Name: name, Value: value });
        },
        _useSearchParams: function (params) {
            var paramValues = this._paramValues;
            if (paramValues) {
                this._searchParamValues = paramValues;
                this._paramValues = null;
            }
            else
                paramValues = this._searchParamValues;
            if (paramValues) {
                var values = Array.clone(params.ExternalFilter);
                for (var i = 0; i < paramValues.length; i++) {
                    var v = paramValues[i];
                    Array.add(values, { Name: v.Name, Value: v.NewValue });
                }
                params.ExternalFilter = values;
            }
        },
        _forceSync: function () {
            var that = this;
            if (that._skipSync || _touch && that.get_isForm())
                that._skipSync = false;
            else
                that._syncKey = that.get_selectedKey();
        },
        sync: function (keyValues) {
            this._clearCache();
            if (keyValues != null) {
                if (!Array.isInstanceOfType(keyValues))
                    keyValues = [keyValues];
                var row = [];
                for (var i = 0; i < this._keyFields.length; i++) {
                    var f = this._keyFields[i];
                    row[this._keyFields[i].Index] = keyValues[i];
                }
                this._raiseSelectedDelayed = true;
                this._pendingSelectedEvent = true
                this._selectKeyByRow(row);
            }
            this._forceSync()
            this.refreshAndResize();
            this._synced = true;
        },
        _sync: function () {
            var settings = this._viewColumnSettings;
            this._viewColumnSettings = [];
            this.sync();
            this._viewColumnSettings = settings;
        },
        combinedFilter: function () {
            return this._combinedFilter(this.get_filter());
        },
        dataSignature: function () {
            var filter = this.combinedFilter();
            return _serializer.serialize(filter);
        },
        _combinedFilter: function (filter) {
            var that = this,
                advancedSearchFilter;
            if (_touch && that.asearch('active')) {
                advancedSearchFilter = that.asearch('filter');
                if (advancedSearchFilter && advancedSearchFilter.length) {
                    $(filter).each(function () {
                        if (this.startsWith('_quickfind_')) {
                            filter.splice(filter.indexOf(this), 1);
                            return false;
                        }
                    });
                    filter = (filter || []).concat(advancedSearchFilter);
                }
            }
            return filter;
        },
        _createParams: function (filterByPosition) {
            var that = this,
                lc = that.get_lookupContext(),
                viewType = that.get_viewType(),
                extension = that.extension(),
                lookupInfo = that._lookupInfo;

            if (isNullOrEmpty(viewType))
                viewType = that.readContext('GridType');

            if (that.get_searchOnStart() && lookupInfo && lookupInfo.value != null)
                that.set_searchOnStart(false);

            var confirmContext = that.get_confirmContext(),
                api = that._api,
                params = {
                    PageIndex: that.get_pageIndex(), PageSize: that.get_pageSize(), PageOffset: that.get_pageOffset(), SortExpression: that.get_sortExpression(), GroupExpression: that.get_groupExpression(),
                    Filter: that._combinedFilter(that.get_filter()), ContextKey: that.get_id(), /*Cookie: that.get_cookie(),*/ FilterIsExternal: that._externalFilter.length > 0,
                    LookupContextFieldName: lc ? lc.FieldName : null, LookupContextController: lc ? lc.Controller : null, LookupContextView: lc ? lc.View : null,
                    LookupContext: lc, Inserting: that.inserting(), LastCommandName: that.get_lastCommandName(), LastCommandArgument: that.get_lastCommandArgument(),
                    /*SelectedValues: that.get_selectedValues(),*/ExternalFilter: that.get_externalFilter(), /*Transaction: that.get_transaction(),*/
                    DoesNotRequireData: that.get_searchOnStart(), LastView: that.get_lastViewId(), Tag: that.get_tag(), RequiresFirstLetters: that.get_showFirstLetters(),
                    ViewType: viewType, SupportsCaching: Sys.Browser.agent != Sys.Browser.InternetExplorer || Sys.Browser.version > 7 || _app.host != null,
                    SystemFilter: extension ? extension.systemFilter() : null, RequiresRowCount: that._requiresRowCount == true,
                    QuickFindHint: _touch ? that.viewProp('quickFindHint') : null, RequiresPivot: that._requiresPivot == true, PivotDefinitions: that._pivotDefinitions ? that._pivotDefinitions : null
                };
            if (api) {
                if (this._distinctValues)
                    params.Distinct = true;
                params.SupportsCaching = false;
                if (api.DoesNotRequireData)
                    params.DoesNotRequireData = true;
                params.MetadataFilter = api.metadataFilter;
                params.FieldFilter = api.fieldFilter;
                if (api.requiresAggregates == false)
                    params.DoesNotRequireAggregates = true;
                if (api.inserting)
                    params.Inserting = true;
            }
            if (that.tagged('inline-editing'))
                params.DoesNotRequireAggregates = true;

            if (confirmContext) {
                var values = Array.clone(params.ExternalFilter);
                Array.addRange(values, confirmContext.Values);
                params.ExternalFilter = values;
            }
            that._useSearchParams(params);
            if (that._position) {
                if (that._position.changing) {
                    params.PageIndex = that._position.index;
                    params.PageSize = 1;
                    params.Filter = that._position.filter;
                    params.SortExpression = that._position.sortExpression;
                    params.RequiresMetaData = true;
                }
                if (filterByPosition)
                    params.Filter = that._selectedKeyFilter;
            }
            // assign syncKey and clear them up
            params.SyncKey = that._syncKey;
            if (that.useCase('$app'))
                params.RequiresMetaData = true;
            return params;
        },
        _createArguments: function (args, view, values) {
            if (!values)
                values = this._collectFieldValues();
            if (!view)
                view = this.get_viewId();
            if (args.values)
                values = Array.clone(args.values);
            if (this._paramValues) {
                values = Array.clone(values);
                Array.addRange(values, this._paramValues);
                delete this._paramVlaues;
            }
            var actionArgs = {
                CommandName: args.commandName, CommandArgument: args.commandArgument, Path: args.path, LastCommandName: this.get_lastCommandName(), Values:
                    values, ContextKey: this.get_id(), /*Cookie: this.get_cookie(), */Controller: this.get_controller(), View: view, LastView: this.get_lastViewId(), Tag: this.get_tag(), Trigger: args.trigger
            };
            actionArgs.Filter = this._combinedFilter(this.get_filter()); // this.get_filter(); 
            actionArgs.SortExpression = this.get_sortExpression();
            actionArgs.GroupExpression = this.get_groupExpression();
            actionArgs.SelectedValues = this.get_selectedValues();
            actionArgs.ExternalFilter = Array.clone(this.get_externalFilter());
            //actionArgs.Transaction = this.get_transaction();
            if (args.commandName == 'PopulateDynamicLookups')
                for (var i = 0; i < this._allFields.length; i++) {
                    var contextFilter = this.get_contextFilter(this._allFields[i], values);
                    if (contextFilter.length > 0)
                        Array.addRange(actionArgs.ExternalFilter, contextFilter);
                }
            //if (!isNullOrEmpty(actionArgs.Transaction) && !this.get_isModal() && !this.get_filterSource() && actionArgs.CommandName.match(/Insert|Update|Delete/))
            //    actionArgs.Transaction += ':complete';
            actionArgs.SaveLEVs = this._allowLEVs == true;
            return actionArgs;
        },
        _loadPage: function () {
            var that = this,
                survey = that._survey,
                skipInvoke = that._skipInvoke;
            if (that._isBusy)
                that._cancelWSRequest();
            that._delayedLoading = false;
            if (that._source) return;
            if (that.get_mode() != Web.DataViewMode.View) {
                that._allFields = [{ Index: 0, Label: '', DataFormatString: '', AliasIndex: 0, ItemsDataController: that.get_controller(), ItemsNewDataView: that.get_newViewId(), ItemsDataView: that.get_viewId(), _dataView: that, Behaviors: [], format: _field_format, isReadOnly: _field_isReadOnly, isNumber: _field_isNumber, lov: _field_lov }];
                that._fields = that._allFields;
                that._render();
            }
            else {
                if (!skipInvoke) {
                    that._busy(true);
                    that._detachBehaviors();
                    that._showWait();
                }
                var r = that._createParams();
                if (skipInvoke)
                    that._invokeArgs = r;
                else {
                    that._restoreViewVitals(r);
                    var pageArgs = { controller: that.get_controller(), view: that.get_viewId(), request: r };
                    var rules = new _businessRules(that);
                    rules.before({ commandName: 'Select', commandArgument: pageArgs.view });
                    rules.dispose();

                    if (survey && survey.result) {
                        that._onGetPageComplete(survey.result, null);
                        survey.result = null;
                    }
                    else if (that._startPage) {
                        that._onGetPageComplete(that._startPage, null);
                        that._startPage = null;
                    }
                    else
                        that._invoke('GetPage', pageArgs, Function.createDelegate(that, that._onGetPageComplete));
                }
            }
        },
        _cancelWSRequest: function () {
            var webServiceRequest = this._wsRequest, browser = Sys.Browser;
            if (webServiceRequest) {
                this._wsRequest = null;
                if (webServiceRequest.get_executor && !webServiceRequest.completed() && (!browser.agent == browser.InternetExplorer || browser.version > 9))
                    webServiceRequest.get_executor().abort();
            }
        },
        _invoke: function (methodName, params, onSuccess, userContext, onFailure) {
            var that = this,
                servicePath = that.get_servicePath(),
                ensureJSONCompatibility = _app.ensureJSONCompatibility;

            function retry(error, response, method) {
                if (_app._navigated) return;
                var statusCode = error.get_statusCode();
                if (statusCode == 0) {
                    _app.confirm(resourcesData.ConnectionLost,
                        function () {
                            that._wsRequest = Sys.Net.WebServiceProxy.invoke(servicePath, methodName, false, params, onSuccess,
                                retry, userContext);
                        },
                        function () {
                            that._onMethodFailed(error, response, method);
                        });
                }
                else if (statusCode > 0) {
                    // report error only if the status code is real (greater than zero) 
                    that._onMethodFailed(error, response, method);
                    if (onFailure)
                        onFailure();
                }
            }

            that._autoRefresh(true);

            if (params.args)
                ensureJSONCompatibility(params.args.Values);
            if (params.request && params.request.Filter)
                ensureJSONCompatibility(params.request.Filter);
            if (params.request && params.request.ExternalFilter)
                ensureJSONCompatibility(params.request.ExternalFilter);

            if (servicePath.match(/\.asmx/))
                that._wsRequest = Sys.Net.WebServiceProxy.invoke(servicePath, methodName, false, params,
                    function (result, context, method) {
                        if (result && result.ExceptionType)
                            that._onMethodFailed({
                                // err.get_timedOut(), err.get_exceptionType(), err.get_message(), err.get_stackTrace()
                                get_timedOut: function () {
                                    return false;
                                },
                                get_exceptionType: function () {
                                    return result.ExceptionType;
                                },
                                get_message: function () {
                                    return result.Message;
                                },
                                get_stackTrace: function () {
                                    return result.StackTrace;
                                },
                                get_statusCode: function () {
                                    return "HSR"
                                }
                            },
                                null, userContext);
                        else
                            onSuccess(result, context, method);
                    },
                    retry, userContext);
            else
                _app.odp.invoke(that, {
                    url: servicePath + '/' + methodName,
                    method: 'POST',
                    cache: false,
                    dataType: 'text',
                    data: params
                }).done(function (result) {
                    var resultIsString = typeof result === 'string';
                    if (_touch && methodName == 'GetPage' && resultIsString) {
                        if (that.tagged('system-replacegetpagetemplate')) {
                            var dv = that.get_parentDataView(that);
                            dv.session('getPageTemplate', result);
                        }
                        else if (!that.session('getPageTemplate'))
                            that.session('getPageTemplate', result);
                    }
                    var data = resultIsString ? _app.parseJSON(result) : result;// result ? eval('(' + deserializeControllerJson(result) + ')').d : null;
                    if (data && data.RedirectUrl)
                        location.href = data.RedirectUrl;
                    else if (data && data.ExceptionType)
                        that._onMethodFailed({
                            // err.get_timedOut(), err.get_exceptionType(), err.get_message(), err.get_stackTrace()
                            get_timedOut: function () {
                                return false;
                            },
                            get_exceptionType: function () {
                                return data.ExceptionType;
                            },
                            get_message: function () {
                                return data.Message;
                            },
                            get_stackTrace: function () {
                                return data.StackTrace;
                            },
                            get_statusCode: function () {
                                return "HSR"
                            }
                        },
                            null, userContext);
                    else
                        onSuccess(data, userContext);
                }).fail(function (jqXHR, textStatus, error) {
                    retry(
                        { get_statusCode: function () { return jqXHR.status }, get_timedOut: function () { return false }, get_exceptionType: function () { return textStatus }, get_message: function () { return error.message }, get_stackTrace: function () { return error.stack } },
                        jqXHR, methodName);
                });
        },
        _disposeFields: function () {
            if (this._allFields) {
                for (var i = 0; i < this._allFields.length; i++) {
                    var f = this._allFields[i];
                    f._dataView = null;
                    if (f._listOfValues) Array.clear(f._listOfValues);
                }
            }
        },
        _formatViewText: function (text, lowerCase, altText) {
            var vl = this._views.length > 0 ? this._views[0].Label : (this._view ? this._view.Label : '');
            return !isNullOrEmpty(text) ? String.format(text, lowerCase == true ? vl.toLowerCase() : vl) : altText;
        },
        _autoRefresh: function (stop) {
            if (this._refreshInterval > 0) {
                if (this._riTimeout) {
                    clearTimeout(this._riTimeout);
                    this._riTimeout = null;
                }
                if (!stop) {
                    var self = this;
                    this._riTimeout = setTimeout(function () {
                        if (!self.get_isForm())
                            self.sync();
                    }, this._refreshInterval * 1000);
                }
            }
        },
        _clearCache: function (reset) {
            var cachedPages = this._cachedPages,
                extension,
                resetEvent = $.Event('reset.dataview.app'),
                i, p;
            if (cachedPages) {
                for (i = 0; i < cachedPages.length; i++) {
                    p = cachedPages[i];
                    Array.clear(p.rows);
                    delete p.rows;
                }
                Array.clear(cachedPages);
                delete this._cachedPages;
            }
            extension = this.extension();
            if (extension)
                extension.reset(reset == null || reset == true);
            resetEvent.dataView = this;
            $(document).trigger(resetEvent);

        },
        _supportsCaching: function (viewType) {
            return viewType == 'DataSheet' || viewType == 'Grid' && _touch;
        },
        _cacheResult: function (result) {
            var pageSize = result.PageSize;
            var doCaching = result.PageSize < result.Rows.length;
            var viewType = this.get_viewType();
            if (!doCaching && !viewType)
                viewType = this.readContext('GridType');
            if (!doCaching)
                doCaching = this._supportsCaching(viewType);
            if (!doCaching && !viewType && result.Views)
                for (var i = 0; i < result.Views.length; i++) {
                    var v = result.Views[i];
                    if (v.Id == result.View) {
                        doCaching = this._supportsCaching(v.Type);
                        break;
                    }
                }

            if (doCaching && result.SupportsCaching) {
                var rows = result.Rows;
                result.Rows = [];
                var cachedPages = this._cachedPages;
                if (!cachedPages)
                    cachedPages = this._cachedPages = [];
                var pageIndex = result.PageIndex;
                var startIndex = 0;
                var endIndex = pageSize - 1;
                if (pageIndex > 0)
                    if (rows.length <= pageSize * 2) {
                        startIndex = pageSize;
                        endIndex = rows.length - 1;
                    }
                    else {
                        startIndex += pageSize;
                        endIndex += pageSize;
                    }
                // copy the request page back to the result 
                var page = { index: pageIndex, rows: [] };
                for (i = startIndex; i <= endIndex; i++) {
                    var r = rows[i];
                    if (r) {
                        //Array.add(result.Rows, Array.clone(r));
                        result.Rows.push(r.slice(0));
                        //Array.add(page.rows, r);
                        page.rows.push(r);
                    }
                }
                // cache the previous page
                var prevPage = null;
                if (startIndex > 0) {
                    prevPage = { index: pageIndex - 1, rows: [] };
                    for (i = 0; i < startIndex; i++)
                        //Array.add(prevPage.rows, rows[i]);
                        prevPage.rows.push(rows[i]);
                }
                // cache the next page
                var nextPage = null;
                if (endIndex < rows.length - 1) {
                    nextPage = { index: pageIndex + 1, rows: [] };
                    for (i = endIndex + 1; i < rows.length; i++)
                        //Array.add(nextPage.rows, rows[i]);
                        nextPage.rows.push(rows[i]);
                }
                i = 0;
                while (i < cachedPages.length) {
                    var p = cachedPages[i];
                    if (page != null && p.index == page.index) {
                        cachedPages[i] = page;
                        page = null;
                    }
                    else if (prevPage != null && p.index == prevPage.index) {
                        cachedPages[i] = prevPage;
                        prevPage = null;
                    }
                    else if (nextPage != null && p.index == nextPage.index) {
                        cachedPages[i] = nextPage;
                        nextPage = null;
                    }
                    else
                        i++;
                }
                if (page || prevPage || nextPage) {
                    if (cachedPages.length > 100) {
                        cachedPages.splice(0, 3);
                        //Array.removeAt(cachedPages, 0);
                        //Array.removeAt(cachedPages, 0);
                        //Array.removeAt(cachedPages, 0);
                    }
                    if (page)
                        //Array.add(cachedPages, page);
                        cachedPages.push(page);
                    if (prevPage)
                        //Array.add(cachedPages, prevPage);
                        cachedPages.push(prevPage);
                    if (nextPage)
                        //Array.add(cachedPages, nextPage);
                        cachedPages.push(nextPage);
                }
            }
        },
        _onGetPageComplete: function (result, context) {
            var that = this,
                serverNewRow = result.NewRow,
                newRow = serverNewRow,
                resultFields = result.Fields || [],
                i, expressions;
            if (that._syncKey)
                that._syncKey = null;
            that._busy(false);
            configureDefaultValues(result, getPagePropertiesWithEmptyArrayDefault, []);
            if (Sys.Services && Sys.Services.AuthenticationService && Sys.Services.AuthenticationService.get_isLoggedIn && Sys.Services.AuthenticationService.get_isLoggedIn() && !result.IsAuthenticated) {
                //window.location.reload();
                location.replace(_app.unanchor(location.href));
                return;
            }
            if (!that._fields)
                that._viewId = result.View;
            if (that._containerIsHidden) {
                Sys.UI.DomElement.setVisible(that._container, true);
                that._containerIsHidden = false;
            }
            that.set_tag(result.Tag);
            if (result.FirstLetters != '')
                that._firstLetters = result.FirstLetters ? result.FirstLetters.split(/,/) : null;
            var positionChanged = that._position && that._position.changed;
            that._cacheResult(result);
            if (that._pageIndex < 0 || positionChanged) {
                if (this._pageIndex == -1 || positionChanged) {
                    _businessRules.reset(that._controller);
                    that._disposeFields();
                    expressions = that._expressions = result.Expressions;
                    that._detachBehaviors();
                    that._allFields = resultFields;
                    that._mapOfAllFields = {};
                    that._fields = [];
                    var selectedKeyMap = [];
                    if (that._keyFields && this._selectedKey.length > 0) {
                        for (i = 0; i < that._keyFields.length; i++)
                            selectedKeyMap[i] = { 'name': this._keyFields[i].Name, 'value': that._selectedKey[i] };
                        that._selectedKey = [];
                    }
                    that._keyFields = [];
                    var hasStatusField = false,
                        hasMapFields = false,
                        mapFields = [],
                        keyMapFields = 0,
                        viewType;

                    $(result.Views).each(function () {
                        var v = this;
                        if (result.View == v.Id) {
                            v.HeaderText = result.ViewHeaderText;
                            if (that.tagged('view-type-inline-editor')) {
                                v.Type = 'Form';
                                v.HeaderText = null;
                                that._inlineEditor = true;
                                that.tag('modal-fit-content');
                            }
                            else
                                v.Layout = result.ViewLayout;
                            viewType = v.Type;
                        }
                        if (v.ShowInSelector == null)
                            v.ShowInSelector = true;
                    });
                    that._views = result.Views;

                    var hasPrimaryKey;
                    if (viewType == 'Form') {
                        for (i = 0; i < resultFields.length; i++)
                            if (resultFields[i].IsPrimaryKey) {
                                hasPrimaryKey = true;
                                break;
                            }
                        if (!hasPrimaryKey)
                            resultFields.push({ Name: 'sys_pk_', ReadOnly: true, Type: 'Int32', IsPrimaryKey: true, Hidden: true });
                    }

                    i = 0;
                    $(resultFields).each(function () {
                        var f = this;
                        that._mapOfAllFields[f.Name] = f;
                        f.OriginalIndex = f.Index = i++;
                        configureDefaultProperties(f);
                        if (f.Name == 'Status')
                            hasStatusField = true;
                    });

                    if (!hasStatusField)
                        that._mapOfAllFields['Status'] = resultFields[resultFields.length] = { 'Name': 'Status', 'ReadOnly': true, 'Type': 'String', 'AllowNulls': true, 'Hidden': true, 'Index': resultFields.length, '_system': true };

                    var displayFieldList = _app._commandLine.match(/\W_display=(.+?)(&|$)/),
                        field,
                        filterSource,
                        fieldName;
                    for (i = 0; i < resultFields.length; i++) {
                        field = resultFields[i];
                        fieldName = field.Name;
                        filterSource = field.DataViewFilterSource;
                        field.tagged = _field_tagged;
                        field.is = _field_is;
                        field.tag = _field_tag;
                        var fa = !isNullOrEmpty(field.AliasName) ? this.findField(field.AliasName) : null,
                            copyInfo, copyField;
                        field.AliasIndex = fa ? fa.Index : i;
                        if (fa)
                            fa.OriginalIndex = field.Index;
                        if (field.ItemsDataController) {
                            if (!field.AliasName && !field._autoAlias && field.Type != 'String' && field.ItemsDataValueField != field.ItemsDataTextField && viewType == 'Form')
                                field._autoAlias = true;
                            if (field._autoAlias) {
                                var lookupFieldAutoAlias = { Name: fieldName + '_auto_alias_', ReadOnly: true, Type: 'String', Index: resultFields.length, Label: field.Label, AllowNulls: true, Items: [], htmlEncode: true, Hidden: true };
                                configureDefaultProperties(lookupFieldAutoAlias);
                                lookupFieldAutoAlias.AliasIndex = lookupFieldAutoAlias.Index;
                                field.AliasIndex = lookupFieldAutoAlias.Index;
                                resultFields.push(lookupFieldAutoAlias);
                                that._mapOfAllFields[lookupFieldAutoAlias.Name] = lookupFieldAutoAlias;
                                if (field.Items && newRow) {
                                    var newVal = newRow[field.Index];
                                    if (newVal) $(field.Items).each(function (i, v) { if (v[0] == newVal) newRow[lookupFieldAutoAlias.Index] = v[1]; });
                                }
                            }
                        }
                        if ((!field.Hidden || displayFieldList) && this._fieldIsInExternalFilter(field) && this.get_hideExternalFilterFields()) {
                            var isHidden = field.Hidden;
                            //if (_app.touch && that.get_viewType() == 'Form')
                            //    field.TextMode = 4;
                            //else
                            field.Hidden = true;
                            if (field.Copy)
                                while (copyInfo = _app._fieldMapRegex.exec(field.Copy)) {
                                    copyField = this.findField(copyInfo[1]);
                                    if (copyField && copyField.ReadOnly)
                                        copyField.Hidden = true;
                                }
                            if (displayFieldList && Array.indexOfCaseInsensitive(displayFieldList[1].split(','), fieldName) != -1) {
                                if (!isHidden)
                                    field.Hidden = false;
                                if (this.inserting()) {
                                    var valueRegex = new RegExp(String.format('\\W{0}=(.*?)(&|$)', fieldName));
                                    var valueMatch = _app._commandLine.match(valueRegex);
                                    if (valueMatch) {
                                        if (!newRow)
                                            newRow = [];
                                        newRow[i] = decodeURIComponent(valueMatch[1]);
                                    }
                                }
                            }
                        }
                        if (filterSource && field.Type == 'DataView') {
                            var visExp,
                                viewId = result.View,
                                visExpTest = '!$row.' + filterSource + '._ready||$row.' + filterSource + '._selected';
                            if (!expressions)
                                expressions = this._expressions = [];
                            $(expressions).each(function () {
                                var exp = this;
                                if (exp.Scope == 3 && exp.Target == field.Name && exp.ViewId == viewId)
                                    visExp = exp;
                                return !!visExp;
                            });
                            if (visExp)
                                visExp.Test = visExpTest + '&&(' + visExp.Test + ')';
                            else
                                expressions.push({ Type: 1, Scope: 3, Target: fieldName, ViewId: viewId, Test: visExpTest });
                        }
                        field.Behaviors = [];
                    }
                    if (_app.newValues) {
                        if (!this._ditto)
                            this._ditto = [];
                        this._ditto = this._ditto.concat(_app.newValues);
                        _app.newValues = null;
                    }
                    if (newRow && this._ditto)
                        for (i = 0; i < resultFields.length; i++) {
                            field = resultFields[i];
                            if (newRow[i] != null)
                                for (var j = 0; j < this._ditto.length; j++)
                                    if (this._ditto[j].name == field.Name && !this._ditto[j].duplicated) {
                                        Array.removeAt(this._ditto, j);
                                        break;
                                    }
                        }
                    this._ignoreNewRow = true;
                    this._hasDynamicLookups = false;
                    this._requiresConfiguration = false;
                    var colIndex = 0;
                    for (i = 0; i < resultFields.length; i++) {
                        field = resultFields[i];
                        field._dataView = this;
                        if (!field.Hidden) Array.add(this._fields, field);
                        if (field.IsPrimaryKey) {
                            Array.add(this._keyFields, field);
                            for (j = 0; j < selectedKeyMap.length; j++) {
                                if (selectedKeyMap[j].name == field.Name) {
                                    Array.add(this._selectedKey, selectedKeyMap[j].value);
                                    break;
                                }
                            }
                        }
                        //if (isNullOrEmpty(field.HeaderText)) field.HeaderText = field.Label;
                        //if (isNullOrEmpty(field.HeaderText)) field.HeaderText = field.Name;
                        field.FilterType = 'Number';
                        switch (field.Type) {
                            case 'Time':
                            case 'String':
                                field.FilterType = 'Text';
                                break;
                            case 'Date':
                            case 'DateTime':
                            case 'DateTimeOffset':
                                field.FilterType = 'Date';
                                break;
                            case 'Boolean':
                                field.FilterType = 'Boolean'
                                break;
                        }
                        if (field.OnDemand) {
                            var blobFieldName = field.Name,
                                lengthField = that.findField(blobFieldName + 'Length') || that.findField(blobFieldName + 'LENGTH') || that.findField(blobFieldName + 'length') || that.findField('Length') || that.findField('LENGTH') || that.findField('length');
                            if (lengthField)
                                lengthField._smartSize = true;
                            if (field.OnDemandStyle == 0 && !this.inserting()) {
                                if (!that._headerImageField)
                                    that._headerImageField = field;
                                if (field.tagged('header-image'))
                                    that._headerImageField = field;
                            }
                        }
                        var searchOptions = field.SearchOptions;
                        if (searchOptions)
                            searchOptions = searchOptions.replace(/\$quickfind(\w+)?\s*/gi, '');
                        if (!isBlank(searchOptions) && !(__tf != 4)) {
                            searchOptions = searchOptions.replace(/\s+/g, ',').split(/,/);
                            field.AllowAutoComplete = !Array.contains(searchOptions, '$disableautocomplete');
                            if (!field.AllowAutoComplete)
                                Array.remove(searchOptions, '$disableautocomplete');
                            field.AllowSamples = !Array.contains(searchOptions, '$disablesamples');
                            if (!field.AllowSamples)
                                Array.remove(searchOptions, '$disablesamples');
                            field.AllowMultipleValues = !Array.contains(searchOptions, '$disablemultiplevalues');
                            if (!field.AllowMultipleValues)
                                Array.remove(searchOptions, '$disablemultiplevalues');
                            field.AutoCompleteAnywhere = Array.contains(searchOptions, '$autocompleteanywhere');
                            if (field.AutoCompleteAnywhere) {
                                Array.remove(searchOptions, '$autocompleteanywhere');
                                resultFields[field.AliasIndex].AutoCompleteAnywhere = true;
                            }
                            j = 0;
                            var includeMissingOptions = false;
                            while (j < searchOptions.length) {
                                var so = searchOptions[j] = searchOptions[j].trim();
                                if (so == '*')
                                    includeMissingOptions = true;
                                if (so.length > 0 && so != '*')
                                    j++;
                                else
                                    Array.removeAt(searchOptions, j);
                            }
                            if (includeMissingOptions && searchOptions.length > 0) {
                                var filterDef = resourcesDataFilters[field.FilterType]
                                this._enumerateMissingSearchOptions(searchOptions, filterDef.List);
                            }
                            if (Array.contains(searchOptions, '$in') && !Array.contains(searchOptions, '$notin'))
                                Array.add(searchOptions, '$notin');
                            field.SearchOptions = searchOptions.length == 0 ? null : searchOptions;
                            // this line is for compatibility with legacy projects only
                            if (field.SearchOptions && _touch && field.OriginalIndex != field.Index)
                                resultFields[field.OriginalIndex].SearchOptions = field.SearchOptions;
                        }
                        else
                            field.SearchOptions = null;
                        field.format = _field_format;
                        field.isReadOnly = _field_isReadOnly;
                        field.isNumber = _field_isNumber;
                        field.lov = _field_lov;
                        field.text = _field_text;
                        field.trim = _field_trim;
                        field.htmlEncode = _field_htmlEncode;
                        _field_prepareDataFormatString(this, field);
                        var itemsStyle = field.ItemsStyle,
                            itemsDataController = field.ItemsDataController;
                        if (field.Type == 'Boolean') {
                            if (field.Items.length == 0) {
                                field.Items = Array.clone(field.AllowNulls ? resourcesData.BooleanOptionalDefaultItems : resourcesData.BooleanDefaultItems);
                                if (!itemsStyle)
                                    itemsStyle = field.ItemsStyle = resourcesData.BooleanDefaultStyle;
                            }
                            else
                                $(field.Items).each(function () {
                                    var v = this[0];
                                    if (v && typeof v == 'string')
                                        this[0] = v == 'true';
                                });
                        }
                        if (field.Items && field.Items.length > 0 && (field.AllowNulls && !field.ItemsTargetController || itemsStyle == 'DropDownList' && !itemsDataController) && !isNullOrEmpty(field.Items[0][0]) && itemsStyle != 'CheckBoxList' && !field.tagged('lookup-null-value-none'))
                            Array.insert(field.Items, 0, [null, field.tagged('lookup-any-value') ? resourcesData.AnyValue : resourcesData.NullValueInForms]);
                        if (itemsStyle) {
                            if (field.ContextFields && itemsStyle != 'Lookup' && itemsStyle != 'AutoComplete' && itemsDataController) {
                                this._hasDynamicLookups = true;
                                field.ItemsAreDynamic = true;
                            }
                            else if (itemsStyle == 'UserNameLookup') {
                                field.ItemsStyle = 'Lookup';
                                field.ItemsDataController = 'aspnet_Membership';
                                field.ItemsDataTextField = 'UserUserName';
                                field.ItemsDataValueField = 'UserUserName';
                                field.ItemsValueSyncDisabled = true;
                                if (hasAccessToMembership())
                                    field.ItemsNewDataView = 'createForm1';
                                else {
                                    field.ItemsDataView = 'lookup';
                                    field.ItemsDataTextField = 'UserName';
                                    field.ItemsDataValueField = 'UserName';
                                    field.Tag = 'lookup-details-hidden';
                                }
                                //if (Web.Menu.findNode('/Membership.aspx'))
                                //    field.ItemsNewDataView = 'createForm1';
                            }
                            else if (itemsStyle == 'UserIdLookup') {
                                field.ItemsStyle = 'Lookup';
                                field.ItemsDataController = 'aspnet_Membership';
                                field.ItemsDataTextField = 'UserUserName';
                                field.ItemsDataValueField = 'UserId';
                                if (hasAccessToMembership())
                                    field.ItemsNewDataView = 'createForm1';
                                else {
                                    field.ItemsDataView = 'lookup';
                                    field.ItemsDataTextField = 'UserName';
                                    field.Tag = 'lookup-details-hidden';
                                }
                                //if (Web.Menu.findNode('/Membership.aspx'))
                                //    field.ItemsNewDataView = 'createForm1';
                            }
                            if (that._inlineEditor && _touch.pointer('mouse'))
                                if (itemsStyle == 'RadioButtonList' || itemsStyle == 'ListBox') {
                                    field.OriginalItemsStyle = itemsStyle;
                                    field.ItemsStyle = 'DropDownList';
                                }
                        }
                        if (!isNullOrEmpty(field.ToolTip))
                            field.ToolTip = _app.htmlAttributeEncode(field.ToolTip);
                        if (!field.Watermark && !field.AllowNulls && (!field.Items || !field.Items.length))
                            field.Watermark = resourcesValidator.Required;
                        if (!isNullOrEmpty(field.Configuration))
                            this._requiresConfiguration = true;
                        if (field.AllowLEV) this._allowLEVs = true;
                        if (field.TextMode == 2 && isNullOrEmpty(field.Editor)) {
                            if (!_touch)
                                if (Sys.Extended.UI.HtmlEditorExtenderBehavior || typeof CKEDITOR != 'undefined')
                                    field.ClientEditor = 'Web$DataView$RichText';
                                else
                                    field.Editor = 'RichEditor';
                            field.HtmlEncode = false;
                        }

                        if (field.Editor)
                            if (this._executeClientEditorFactories(field)) {
                                field.ClientEditor = field.Editor;
                                field.Editor = null;
                            }
                            else
                                field.EditorId = String.format('{0}_Item{1}', this.get_id(), field.Index);
                        field.ColIndex = !field.Hidden ? colIndex++ : -1;
                        var wdvg = _app.Geo;
                        if (field.tagged('header-text'))
                            that._headerField = field;
                        if (field.tagged('modified-', 'created-') && field.tagged('modified-latitude', 'modified-longitude', 'modified-coords', 'created-latitude', 'created-longitude', 'created-coords')) {
                            if (!wdvg || wdvg.latitude < 0) {
                                setTimeout(function () {
                                    wdvg = _app.Geo = { latitude: -1, longitude: -1 };
                                    var geolocation = navigator.geolocation;
                                    if (geolocation)
                                        geolocation.watchPosition(function (position) {
                                            wdvg.acquired = true;
                                            wdvg.latitude = position.coords.latitude;
                                            wdvg.longitude = position.coords.longitude;
                                        },
                                            function (error) {
                                                wdvg.error = error;
                                                wdvg.acquired = false;
                                            });
                                }, 1500);
                            }
                            field.TextMode = 4;
                        }
                        if (!hasMapFields)
                            if (field.tagged('map-') && !field.tagged('map-none'))
                                hasMapFields = true;
                            else {
                                var mf = field.Name.match(/\b(address|city|state|region|postal(.*?)code|zip(.*?)code|zip|country)\b/i);
                                if (mf && !field.tagged('map-none')) {
                                    mf = mf[1].toLowerCase();
                                    mapFields.push({ field: field, tag: mf });
                                    if (mf.match(/address|city/))
                                        keyMapFields++;
                                }
                            }
                    }
                    this.untag('supports-view-style-map')
                    if (!hasMapFields && keyMapFields == 2) {
                        hasMapFields = true;
                        $(mapFields).each(function () {
                            this.field.tag('map-' + this.tag);
                            if (this.tag == 'address')
                                this.field.tag('action-location');
                        });
                    }
                    if (hasMapFields)
                        this.tag('supports-view-style-map');
                    if (result.LEVs) this._recordLEVs(result.LEVs);
                    //$(result.Views).each(function () {
                    //    var v = this;
                    //    if (result.View == v.Id) {
                    //        v.HeaderText = result.ViewHeaderText;
                    //        v.Layout = result.ViewLayout;
                    //    }
                    //    if (v.ShowInSelector == null)
                    //        v.ShowInSelector = true;
                    //});
                    //that._views = result.Views;
                    var viewTags = that.get_view().Tags;
                    if (viewTags)
                        that.tag(viewTags);
                    if (that.tagged('header-image-none'))
                        that._headerImageField = null;
                    that._view = null;
                    if (!this._lastViewId && !this.get_isForm()/* this.get_view().Type != 'Form'*/)
                        this._lastViewId = result.View;
                    that._actionGroups = result.ActionGroups ? result.ActionGroups : [];
                    that._statusBar = result.StatusBar;
                    var whenTest = /^(true|false)\:(.+)$/,
                        idCounter = 0;
                    that._actionColumn = null;
                    var dynamicActions = that._dynamicActions = {};
                    for (i = 0; i < this._actionGroups.length; i++) {
                        var ag = this._actionGroups[i];
                        ag.maxTextLength = 0;
                        ag.groupText = [];
                        //if (ag.Scope == 'Grid' && this.get_isTree())
                        //    Array.insert(ag.Actions, 0, { 'CommandName': 'Open' });
                        var agt = resourcesActionsScopes[ag.Scope];
                        if (agt && agt._Self) {
                            var ast = agt._Self[ag.HeaderText];
                            if (ast) ag.HeaderText = ast.HeaderText;
                        }
                        if (ag.Scope == 'ActionColumn')
                            this._actionColumn = !isNullOrEmpty(ag.HeaderText) ? ag.HeaderText : resources.Grid.ActionColumnHeaderText;
                        if (!ag.Id)
                            ag.Id = 'auto' + idCounter++;
                        ag.CssClassEx = String.format('Actions-g-{0} ', ag.Id);
                        for (j = 0; j < ag.Actions.length; j++) {
                            var action = ag.Actions[j];
                            if (!action.Id)
                                action.Id = 'auto' + idCounter++;
                            if (action.CausesValidation == null)
                                action.CausesValidation = true;
                            if (action.CommandName == null)
                                action.CommandName = '';
                            action.Path = String.format('{0}/{1}', ag.Id, action.Id);
                            var confirmation = action.Confirmation;
                            if (action.CommandName == 'Search') {
                                this._hasSearchAction = action.Path;
                                this._hasSearchShortcut = confirmation && confirmation.match(/_shortcut\s*=\s*true/);
                            }
                            action.CssClassEx = String.format('Actions-g-{0}-a-{1} ', ag.Id, action.Id);
                            if (agt && isNullOrEmpty(action.HeaderText)) {
                                var at = agt[action.CommandName];
                                if (at) {
                                    var at2;
                                    if (at.CommandArgument) {
                                        at2 = at.CommandArgument[action.CommandArgument];
                                        if (at2) at = at2;
                                    }
                                    if (at.WhenLastCommandName) {
                                        at2 = at.WhenLastCommandName[action.WhenLastCommandName];
                                        if (at2) at = at2;
                                    }
                                    if (at.Controller) {
                                        at2 = at.Controller[this._controller];
                                        if (at2) at = at2;
                                    }
                                    action.HeaderText = at.HeaderText;
                                    action._autoHeaderText = true;
                                    if (!isNullOrEmpty(at.HeaderText) && at.HeaderText.indexOf('{') >= 0) 
                                        action.HeaderText = at.VarMaxLen != null && result.Views[0].Label.length > at.VarMaxLen ? at.HeaderText2 : this._formatViewText(at.HeaderText);
                                    if (isNullOrEmpty(action.Description))
                                        action.Description = this._formatViewText(at.Description);
                                    if (isNullOrEmpty(confirmation))
                                        action.Confirmation = at.Confirmation;
                                    if (!action.Notify)
                                        action.Notify = at.Notify;
                                }
                                else
                                    action.HeaderText = action.CommandName;
                            }
                            if (isNullOrEmpty(action.WhenView))
                                action.WhenViewRegex = null;
                            else {
                                var m = whenTest.exec(action.WhenView);
                                action.WhenViewRegex = new RegExp(m ? m[2] : action.WhenView);
                                action.WhenViewRegexResult = m ? m[1] != 'false' : true;
                            }
                            if (isNullOrEmpty(action.WhenTag))
                                action.WhenTagRegex = null;
                            else {
                                m = whenTest.exec(action.WhenTag);
                                action.WhenTagRegex = new RegExp(m ? m[2] : action.WhenTag);
                                action.WhenTagRegexResult = m ? m[1] != 'false' : true;
                            }
                            if (isNullOrEmpty(action.WhenHRef))
                                action.WhenHRefRegex = null;
                            else {
                                m = whenTest.exec(action.WhenHRef);
                                action.WhenHRefRegex = new RegExp(m ? m[2] : action.WhenHRef);
                                action.WhenHRefRegexResult = m ? m[1] != 'false' : true;
                            }
                            if (action.HeaderText) {
                                ag.maxTextLength = Math.max(action.HeaderText.length, ag.maxTextLength);
                                ag.groupText.push(action.HeaderText);
                            }
                            if (action.WhenClientScript) {
                                dynamicActions[ag.Scope] = 1
                                dynamicActions[ag.Id] = 1;
                                dynamicActions[action.Path] = 1;
                            }
                        }
                    }
                    var numberOfColumns = 1;
                    var hasColumns = false;
                    var categories = this._categories = result.Categories;
                    this._tabs = [];
                    for (i = 0; i < categories.length; i++) {
                        var c = categories[i];
                        c.Index = i;
                        if (c.Tab == null)
                            c.Tab = '';
                        if (!isNullOrEmpty(c.Tab) && !Array.contains(this._tabs, c.Tab))
                            Array.add(this._tabs, c.Tab);
                        if (c.Flow == 'NewColumn') {
                            if (i > 0) numberOfColumns++;
                            hasColumns = true;
                        }
                        c.ColumnIndex = numberOfColumns - 1;
                        if (c.Id) {
                            var t = $get(String.format('{0}_{1}_{2}', result.Controller, result.View, c.Id));
                            if (t)
                                c.Template = t.innerHTML;
                        }
                        if (c.Floating && isNullOrEmpty(c.Template)) {
                            var sb = new Sys.StringBuilder('<div class="FloatingCategoryHeader"></div>');
                            for (j = 0; j < this._allFields.length; j++) {
                                var f = this._allFields[j];
                                if (!f.Hidden && i == f.CategoryIndex)
                                    sb.appendFormat('<div class="FieldPlaceholder">{{{0}}}</div>', f.Name);
                            }
                            c.Template = sb.toString();
                        }
                    }
                    if (this._tabs.length > 0) {
                        for (i = 0; i < categories.length; i++) {
                            c = categories[i];
                            if (isNullOrEmpty(c.Tab)) {
                                if (!isNullOrEmpty(categories[0].Tab))
                                    c.Tab = categories[0].Tab;
                                else {
                                    c.Tab = resources.Form.GeneralTabText;
                                    if (this._tabs[0] != resources.Form.GeneralTabText)
                                        Array.insert(this._tabs, 0, resources.Form.GeneralTabText);
                                }
                            }
                            c.ColumnIndex = 0;
                        }
                        if (this._lastSelectedCategoryTabIndex != null) {
                            var focusedFieldName = this._focusedFieldName;
                            this.set_categoryTabIndex(!(this._lastSelectedCategoryTabIndex >= 0) || focusedFieldName && focusedFieldName.startsWith('_Annotation_') ? this._tabs.length - 1 : 0);
                            delete this._lastSelectedCategoryTabIndex;
                        }
                        else
                            this.set_categoryTabIndex(0);
                        var iconDataView = this.get_parentDataView(this); // this._parentDataViewId ? _app.find(this._parentDataViewId) : this;
                        if (iconDataView._lastClickedIcon == 'Attachment')
                            this.set_categoryTabIndex(this._tabs.length - 1);
                        iconDataView._lastClickedIcon = null;
                        numberOfColumns = 1;
                    }
                    else
                        this.set_categoryTabIndex(-1);
                    this._numberOfColumns = hasColumns && !this._get_template() ? numberOfColumns : 0;
                }
                this._filter = result.Filter;
                this._sortExpression = result.SortExpression;
                this._groupExpression = result.GroupExpression;
                this._updatePageCount(result, positionChanged);
                var pagerButtonCount = this.get_pagerButtonCount(true);
                this._firstPageButtonIndex = Math.floor(result.PageIndex / pagerButtonCount) * pagerButtonCount;
                if (_app.odp)
                    _app.odp.verify(that);
            }
            else if (this._requiresRowCount) {
                this._requiresRowCount = false;
                this._updatePageCount(result, false);
            }
            for (i = 0; i < resultFields.length; i++) {
                field = resultFields[i];
                var aliasField = resultFields[field.AliasIndex];
                if (field.AllowAutoComplete == false)
                    aliasField.AllowAutoComplete = false;
                if (field.AllowSamples == false)
                    aliasField.AllowSamples = false;
                if (field.AllowMultipleValues == false)
                    aliasField.AllowMultipleValues = false;
            }
            that._icons = result.Icons;
            if (!that.inserting()) {
                var identifySelectedRow = false;
                if (that._rows) {
                    if (that._rows.length == 0 || result.Rows.length == 0)
                        delete that._viewColumnSettings;
                    for (i = 0; i < that._rows.length; i++)
                        Array.clear(that._rows[i]);
                    Array.clear(that._rows);
                    identifySelectedRow = that.get_isGrid();
                }
                that._rows = result.Rows;
                if (identifySelectedRow) {
                    that._selectedRowIndex = null;
                    for (i = 0; i < that._rows.length; i++)
                        if (that._rowIsSelected(i)) {
                            that._selectedRowIndex = i;
                            break;
                        }
                }
            }
            that._newRow = newRow = newRow ? newRow : [];
            if (result.Aggregates) that._aggregates = result.Aggregates;
            if (that.get_isForm()/* this.get_view().Type == 'Form'*/ && that._selectedRowIndex == null && that._totalRowCount > 0) {
                that._selectedRowIndex = 0;
                that._selectKeyByRowIndex(0);
            }

            if (positionChanged) {
                that._position.changed = true;
                that._selectKeyByRowIndex(0);
            }
            if (_touch && that.get_isGrid()) {
                var syncMap = _app.controllerSyncMap[that._controller];
                if (!syncMap)
                    syncMap = _app.controllerSyncMap[that._controller] = {};
                syncMap[that._id + '_' + that._viewId] = false;
                that._autoSelect();
            }
            if (serverNewRow && that.odp)
                that.set_selectedKey(that.odp.key(that));
            that._render(true);
            if (!_touch) {
                if (that.get_lookupField())
                    that._adjustLookupSize();
                if (that._isInInstantDetailsMode()) {
                    var size = $common.getClientBounds();
                    var contentSize = $common.getContentSize(document.body);
                    resizeBy(0, contentSize.height - size.height);
                }
                that._saveViewVitals();
                if (that._pendingSelectedEvent) {
                    that._pendingSelectedEvent = false;
                    that.updateSummary();
                }
                that._registerFieldHeaderItems();
                _body_resize();
            }
            that._executeSecondCommand();
            that._autoRefresh();
            if (!_touch)
                that._autoSelect();
            if (!isNullOrEmpty(result.ClientScript))
                eval(result.ClientScript);
            var rules = new _businessRules(that);
            rules.after({ commandName: serverNewRow ? 'New' : 'Select', commandArgument: result.View, view: result.View });
            rules.dispose();
            if (serverNewRow && that.odp)
                that.raiseSelected();
        },
        _updatePageCount: function (result, positionChanged) {
            this._totalRowCount = result.TotalRowCount;
            if (this._position && this._position.count == -1)
                this._position.count = result.TotalRowCount;
            this._pageIndex = result.PageIndex;
            if (!positionChanged) {
                this._pageSize = result.PageSize;
                this._pageCount = Math.floor(result.TotalRowCount / result.PageSize);
                if (result.TotalRowCount % result.PageSize != 0)
                    this._pageCount++;
            }
        },
        _autoSelect: function (rowIndex) {
            var commandName = null,
                commandArgument = null,
                tapAction,
                hideContainer = false;
            if (this.get_autoHighlightFirstRow()) {
                this.set_autoHighlightFirstRow(false);
                commandName = 'Select';
                tapAction = 'highlight';
            }
            if (this.get_autoSelectFirstRow()) {
                this.set_autoSelectFirstRow(false);
                tapAction = 'select';
                var groups = this.get_actionGroups('Grid');
                if (groups.length > 0)
                    for (var i = 0; i < groups[0].Actions.length; i++) {
                        var a = groups[0].Actions[i];
                        if (a.CommandName) {
                            commandName = a.CommandName;
                            commandArgument = a.CommandArgument;
                            hideContainer = !isNullOrEmpty(commandArgument) & !this.get_showModalForms() && rowIndex == null;
                            break;
                        }
                    }
            }
            if (commandName && this.get_viewType() != 'Form' && this._rows.length > 0) {
                if (hideContainer)
                    this._hideContainer();
                var extension = this.extension();
                if (extension)
                    extension._autoSelect = { row: this._rows[rowIndex || 0], action: tapAction };
                else
                    this.executeRowCommand(rowIndex || 0, commandName, commandArgument);
            }
            if (!this._tryFocusDataSheet || this._forceFocusDataSheet) {
                this._tryFocusDataSheet = true;
                if (this.get_isDataSheet() && !this.get_lookupField() & (!this.get_filterSource() || this._forceFocusDataSheet) && this._rows.length > 0 && !_touch)
                    this.executeRowCommand(this.get_selectedKey().length > 0 ? null : 0, 'Select');
                this._forceFocusDataSheet = false;
            }
        },
        _hideContainer: function () {
            this._containerIsHidden = true;
            this._container.style.display = 'none';
        },
        _enumerateMissingSearchOptions: function (searchOptions, filterList) {
            for (var i = 0; i < filterList.length; i++) {
                var fd = filterList[i];
                if (fd && !fd.Hidden)
                    if (fd.List)
                        this._enumerateExpressions(searchOptions, filterList);
                    else
                        if (!Array.contains(searchOptions, fd.Function))
                            Array.add(searchOptions, fd.Function);
            }
        },
        _executeSecondCommand: function (force) {
            if (force) {
                var m = _app._commandLine.match(/_commandName2=(\w+)(.*?_commandArgument2=(.*?)(&|$))?/);
                if (m)
                    this.executeActionInScope([this.get_viewType(), 'ActionBar'], m[1], m[3]);
            }
            else if (this._trySecondCommand) {
                this._trySecondCommand = false;
                if (this.get_viewType() != 'Form' || this._totalRowCount > 0) {
                    var self = this;
                    setTimeout(function () {
                        self._executeSecondCommand(true);
                    }, 50);
                }
            }
        },
        _skipNextInputListenerClickEvent: function () {
            if (Sys.Browser.agent != Sys.Browser.InternetExplorer || Sys.Browser.version >= 9)
                this._skipClickEvent = true;
        },
        _gridViewCellFocus: function (event, rowIndex, colIndex) {
            try {
                var ev = new Sys.UI.DomEvent(event);
                var eventTarget = ev.target;
                if ((eventTarget.tagName == 'A' || eventTarget.parentNode.tagName == 'A') && !this.get_lookupField() || Sys.UI.DomElement.containsCssClass(eventTarget, 'RowSelector')) return false;
                if (eventTarget.tagName == 'SPAN' && eventTarget.className == 'ObjectRef')
                    return false;
                if (this.get_lookupField()) {
                    ev.stopPropagation();
                    ev.preventDefault();
                }
                this._skipNextInputListenerClickEvent();
            }
            catch (ex) {
            }
            this.executeRowCommand(rowIndex, 'Select');
            return true;
        },
        _dataSheetCellFocus: function (event, rowIndex, colIndex) {
            var fc = this._get_focusedCell();
            var inserting = this.inserting();
            if (inserting && rowIndex != -1)
                rowIndex = -1;
            if (colIndex == -1)
                colIndex = fc != null ? fc.colIndex : 0;
            if (this.editing() && fc) {
                this._focusCell(-1, -1, false);
                this._focusCell(rowIndex, colIndex);
                var thisRow = rowIndex == fc.rowIndex || inserting && rowIndex == -1;
                if (!thisRow && !this._updateFocusedRow(fc) || !this._updateFocusedCell(fc)) {
                    this._focusCell(rowIndex, colIndex, false);
                    this._focusCell(fc.rowIndex, fc.colIndex);
                }
                else if (thisRow && (rowIndex != this._selectedRowIndex && !inserting))
                    this.cancelDataSheetEdit();
                this._skipNextInputListenerClickEvent();
                return;
            }
            if (!event)
                this._skipNextInputListenerClickEvent();
            else if (!this._gridViewCellFocus(event, rowIndex, colIndex))
                return;
            if (fc != null && fc.rowIndex == rowIndex && fc.colIndex == colIndex && !this.editing() && !this.get_lookupField()) {
                if (document.selection)
                    document.selection.clear();
                if (this._skipEditOnClick != true && this._allowEdit())
                    this.editDataSheetRow(fc.rowIndex);
            }
            else
                this._startInputListenerOnCell(rowIndex, colIndex);
            this._skipEditOnClick = false;
        },
        _startInputListenerOnCell: function (rowIndex, colIndex) {
            this._startInputListener();
            this._focusCell(-1, -1, false);
            this._focusCell(rowIndex, colIndex);
            if (!this.get_lookupField()) {
                if (_app._activeDataSheetId != this.get_id()) {
                    var dv = $find(_app._activeDataSheetId);
                    if (dv)
                        dv.cancelDataSheet();
                    _app._activeDataSheetId = this.get_id();
                }
                this._lostFocus = false;
            }
        },
        _startInputListener: function () {
            this._stopInputListener();
            if (!this._inputListenerKeyDownHandler) {
                this._inputListenerKeyDownHandler = Function.createDelegate(this, this._inputListenerKeyDown);
                this._inputListenerKeyPressHandler = Function.createDelegate(this, this._inputListenerKeyPress);
                this._inputListenerClickHandler = Function.createDelegate(this, this._inputListenerClick);
                this._inputListenerDblClickHandler = Function.createDelegate(this, this._inputListenerDblClick);
                this._focusedCell = null;
            }
            $addHandler(document, 'keydown', this._inputListenerKeyDownHandler);
            $addHandler(document, 'keypress', this._inputListenerKeyPressHandler);
            $addHandler(document, 'click', this._inputListenerClickHandler);
            $addHandler(document, 'dblclick', this._inputListenerDblClickHandler);
            this._trackingInput = true;
        },
        _stopInputListener: function () {
            if (!this._trackingInput) return;
            $removeHandler(document, 'keydown', this._inputListenerKeyDownHandler);
            $removeHandler(document, 'keypress', this._inputListenerKeyPressHandler);
            $removeHandler(document, 'click', this._inputListenerClickHandler);
            $removeHandler(document, 'dblclick', this._inputListenerDblClickHandler);
            this._lostFocus = true;
            this._trackingInput = false;
        },
        _allowEdit: function () {
            return this._findActionsByCommandName('Edit').length > 0;
        },
        _allowNew: function () {
            return this._findActionsByCommandName('New').length > 0;
        },
        get_interactiveActionGroups: function () {
            var actionGroups = this.get_actionGroups('Grid');
            Array.addRange(actionGroups, this.get_actionGroups('ActionBar'));
            Array.addRange(actionGroups, this.get_actionGroups('Form'));
            Array.addRange(actionGroups, this.get_actionGroups('ActionColumn'));
            return actionGroups;
        },
        _findActionsByCommandName: function (commandName) {
            var result = [];
            var actionGroups = this.get_interactiveActionGroups();
            for (var i = 0; i < actionGroups.length; i++) {
                actions = actionGroups[i].Actions;
                for (var j = 0; j < actions.length; j++) {
                    var a = actions[j];
                    if (a.CommandName == commandName)
                        Array.add(result, a);
                }
            }
            return result;
        },
        isDynamicAction: function (path) {
            return !!this._dynamicActions[path];
        },
        findAction: function (path) {
            if (path) {
                path = path.split(/\s*\\|\/\s*/g);
                var that = this,
                    groups = that._actionGroups, g,
                    actions, a,
                    groupId = path[0],
                    actionId = path[1],
                    i, j;

                for (i = 0; i < groups.length; i++) {
                    g = groups[i];
                    if (g.Id == groupId) {
                        actions = g.Actions;
                        for (j = 0; j < actions.length; j++) {
                            a = actions[j];
                            if (a.Id == actionId)
                                return a;
                        }
                    }
                }
            }
            return null;
        },
        _inputListenerKeyPress: function (e) {
            if (e.rawEvent && e.rawEvent.charCode == 0) // Firefox fix
                return;
            if (this._lostFocus) return;

            if (this.editing()) {
                if (this._pendingChars)
                    this._pendingChars += String.fromCharCode(e.charCode);
                return;
            }
            if (this._isBusy) return;
            var fc = this._get_focusedCell();
            if (fc == null) return;
            var field = this._fields[fc.colIndex];
            if (field.ReadOnly) return;
            if (!this._allowEdit()) return;
            this.executeRowCommand(fc.rowIndex, 'Select');
            if (!field.isReadOnly()) {
                this._pendingChars = String.fromCharCode(e.charCode);
                this.editDataSheetRow(fc.rowIndex);
            }
            //        e.stopPropagation();
            //        e.preventDefault();
        },
        cancelDataSheet: function () {
            if (this.get_isDataSheet()) {
                this._focusCell(-1, -1, false);
                this._stopInputListener();
                this.set_ditto(null);
                _app._activeDataSheetId = null;
                this.cancelDataSheetEdit();
                this._lostFocus = false;
                this._focusedCell = null;
            }
        },
        _inputListenerKeyDown: function (e) {
            //if (this.editing()) return;
            if (this._lookupIsActive) return;
            if (this._lostFocus) return;
            if (_web.HoverMonitor._instance.get_isOpen()) return;
            if (this._isBusy) {
                if (this._pendingChars)
                    return;
                e.preventDefault();
                e.stopPropagation();
                return;
            }
            if (this._isBusy) return;
            var fc = this._get_focusedCell();
            if (fc == null) return;
            var fc2 = { 'rowIndex': fc.rowIndex, 'colIndex': fc.colIndex };
            var handled = false;
            var causesRender = false;
            var pageSize = this.get_pageSize();
            if (this._rows.length < pageSize)
                pageSize = this._rows.length;
            switch (e.keyCode) {
                case 83: // Ctrl+S
                case Sys.UI.Key.enter:
                    if (e.keyCode == 83 && !e.ctrlKey) return;
                    if (this.editing()) {
                        var tagName = e.target && e.target.tagName;
                        if ((tagName == 'TEXTAREA' || tagName == 'A') && !e.ctrlKey)
                            return;
                        //                    this.executeRowCommand(fc.rowIndex, 'Update', null, true);
                        //                    if (!this._valid)
                        //                        return;
                        handled = true;
                        this._updateFocusedRow(fc)
                        e.preventDefault();
                        e.stopPropagation();
                        return;
                        //                    if (!this._updateFocusedRow(fc)) {
                        //                        e.preventDefault();
                        //                        e.stopPropagation();
                        //                        return;
                        //                    }
                    }
                    if (e.ctrlKey && !this.editing() || this.get_lookupField()) {
                        handled = true;
                        this.executeRowCommand(fc.rowIndex, 'Select');
                    }
                    else if (e.shiftKey) {
                        if (fc2.rowIndex > 0)
                            fc2.rowIndex--;
                    }
                    else {
                        if (this._moveFocusToNextRow(fc2, pageSize))
                            handled = true;
                        //                    if (fc2.rowIndex < pageSize - 1)
                        //                        fc2.rowIndex++;
                    }
                    break;
                case Sys.UI.Key.down:
                    if (this.editing() || e.ctrlKey) return;
                    if (this._moveFocusToNextRow(fc2, pageSize))
                        handled = true;
                    break;
                case Sys.UI.Key.up:
                    if (this.editing() || e.ctrlKey) return;
                    if (fc2.rowIndex > 0)
                        fc2.rowIndex--;
                    else {
                        if (this._pageOffset == 0 && this.get_pageIndex() == 0) {
                            this._pageOffset = null;
                            handled = true;
                        }
                        else if (this._pageOffset == null) {
                            if (this.get_pageIndex() > 0)
                                this._pageOffset = -1;
                        }
                        else
                            this._pageOffset--;
                        handled = true;
                        if (this._pageOffset == -pageSize) {
                            this._pageOffset = null;
                            if (this.get_pageIndex() > 0)
                                this.goToPage(this.get_pageIndex() - 1);
                        }
                        else
                            this.goToPage(this.get_pageIndex());
                    }
                    break;
                case Sys.UI.Key.tab:
                case Sys.UI.Key.right:
                case Sys.UI.Key.left:
                    var allowRefresh = true;
                    if ((e.keyCode == Sys.UI.Key.right || e.keyCode == Sys.UI.Key.left) && this.editing()) return;
                    if (!e.shiftKey && e.target.parentNode.className == 'Date')
                        return;
                    if (e.shiftKey && e.target.id && e.target.id.match(/\$Time\d+/))
                        return;
                    var lastPageOffset = this._pageOffset;
                    if (e.shiftKey || e.keyCode == Sys.UI.Key.left) {
                        if (fc2.colIndex > 0) {
                            fc2.colIndex--;
                            if (e.keyCode == Sys.UI.Key.tab)
                                while (fc2.colIndex > 0 && this._fields[fc2.colIndex].isReadOnly())
                                    fc2.colIndex--;
                        }
                        else if (this.editing())
                            handled = true;
                    }
                    else if (fc2.colIndex < this._fields.length - 1) {
                        fc2.colIndex++;
                        if (e.keyCode == Sys.UI.Key.tab)
                            while (fc2.colIndex < this._fields.length - 1 && this._fields[fc2.colIndex].isReadOnly())
                                fc2.colIndex++;
                    }
                    else {
                        if (this.editing()) {
                            if (!this._updateFocusedRow(fc, e.keyCode == Sys.UI.Key.tab)) {
                                e.preventDefault();
                                e.stopPropagation();
                                return;
                            }
                            else
                                allowRefresh = false;
                            handled = true;
                        }
                        if (allowRefresh && this._moveFocusToNextRow(fc2, pageSize))
                            handled = true;
                        if (fc2.rowIndex != fc.rowIndex || this._pageOffset != lastPageOffset) {
                            fc2.colIndex = 0;
                            handled = false;
                        }
                    }
                    if (allowRefresh && this.editing())
                        causesRender = true;
                    break;
                case Sys.UI.Key.home:
                    if (this.editing()) return;
                    if (e.ctrlKey) {
                        if (this.get_pageIndex() > 0) {
                            handled = true;
                            this._pageOffset = 0;
                            this.goToPage(0);
                            fc.rowIndex = 0;
                            fc.colIndex = 0;
                        }
                        else {
                            fc2.rowIndex = 0;
                            fc2.colIndex = 0;
                        }
                    }
                    else
                        fc2.colIndex = 0;
                    break;
                case Sys.UI.Key.end:
                    if (this.editing()) return;
                    if (e.ctrlKey) {
                        handled = true;
                        fc.colIndex = this._fields.length - 1;
                        fc.rowIndex = this._totalRowCount % this.get_pageSize() - 1;
                        if (fc.rowIndex < 0)
                            fc.rowIndex = this.get_pageSize();
                        this._pageOffset = null;
                        this.goToPage(this.get_pageCount() - 1);
                    }
                    else
                        fc2.colIndex = this._fields.length - 1;
                    break;
                case Sys.UI.Key.pageUp:
                    if (this.editing()) return;
                    handled = true;
                    if (this.get_pageIndex() > 0) {
                        this.goToPage(this.get_pageIndex() - 1);
                    }
                    else if (this._pageOffset != null) {
                        this._pageOffset = null;
                        this.goToPage(this.get_pageIndex());
                    }
                    break;
                case Sys.UI.Key.pageDown:
                    if (this.editing()) return;
                    handled = true;
                    if (this.get_pageIndex() < this.get_pageCount() - 1)
                        this.goToPage(this.get_pageIndex() + 1);
                    break;
                case Sys.UI.Key.esc:
                    if (!this.cancelDataSheetEdit())
                        this.cancelDataSheet();
                    handled = true;
                    break;
                case Sys.UI.Key.del:
                    if (this.editing() || e.shiftKey || e.altKey) return;
                    handled = true;
                    if (e.ctrlKey)
                        this.deleteDataSheetRow();
                    else {
                        this._pendingChars = '';
                        this.editDataSheetRow(fc.rowIndex);
                    }
                    break;
                case 45: /* Insert */
                    if (!this.editing()) {
                        handled = true;
                        if (this._allowNew())
                            this.newDataSheetRow();
                    }
                    break;
                case 32: /* space */
                    if (e.ctrlKey && this.multiSelect() && !this.inserting()) {
                        handled = true;
                        this.toggleSelectedRow(fc.rowIndex);
                    }
                    else
                        return;
                    break;
                case 113: /* F2 */
                    if (this.editing()) return;
                    handled = true;
                    if (this._allowEdit())
                        this.editDataSheetRow(fc.rowIndex);
                    break;
            }
            if ((fc.rowIndex != fc2.rowIndex || fc.colIndex != fc2.colIndex) && !handled) {
                this._focusCell(fc.rowIndex, fc.colIndex, false);
                this._focusCell(fc2.rowIndex, fc2.colIndex, true);
                handled = true;
            }
            if (handled) {
                e.preventDefault();
                e.stopPropagation();
            }
            if (causesRender) {
                if (!this._updateFocusedCell(fc)) {
                    this._focusCell(fc2.rowIndex, fc2.colIndex, false);
                    this._focusCell(fc.rowIndex, fc.colIndex, true);
                }
            }
        },
        _updateFocusedCell: function (fc) {
            var values = this._collectFieldValues();
            var valid = this._validateFieldValues(values, true, fc);
            var field = this._fields[fc.colIndex];
            if (valid) {
                var doRefresh = true;
                if (field.Index < values.length && values[field.Index].Modified)
                    doRefresh = !this._performValueChanged(field.Index);
                if (doRefresh)
                    this.refresh(true);
            }
            else if (field.Behaviors)
                for (var i = 0; i < field.Behaviors.length; i++) {
                    var b = field.Behaviors[i];
                    if (AjaxControlToolkit.CalendarBehavior.isInstanceOfType(b) && b.get_isOpen()) {
                        b.hide();
                        b.show();
                    }
                }
            return valid;
        },
        _updateFocusedRow: function (fc, saveAndNew) {
            _app.showMessage();
            this._syncFocusedCell = this.inserting();
            this._lastFocusedCell = fc;
            this._skipSync = true;
            this.executeRowCommand(fc.rowIndex, this._syncFocusedCell ? 'Insert' : 'Update', null, true);
            if (this._valid)
                this._saveAndNew = saveAndNew;
            return this._valid;
        },
        _get_selectedDataRowIndex: function (rowIndex) {
            return this.get_pageIndex() * this.get_pageSize() + this.get_pageOffset() + (rowIndex != null ? rowIndex : this._selectedRowIndex);
        },
        executeActionInScope: function (scopes, commandName, commandArgument, rowIndex, test) {
            if (rowIndex == null)
                rowIndex = this._selectedRowIndex;
            for (var j = 0; j < scopes.length; j++) {
                var scope = scopes[j];
                var actionGroups = this.get_actionGroups(scope);
                if (actionGroups)
                    for (var k = 0; k < actionGroups.length; k++) {
                        var actions = actionGroups[k].Actions;
                        if (actions) {
                            for (var i = 0; i < actions.length; i++) {
                                var action = actions[i];
                                if (action.CommandName == commandName && (isNullOrEmpty(commandArgument) || action.CommandArgument == commandArgument) && this._isActionAvailable(action, rowIndex)) {
                                    if (test != true)
                                        this.executeAction(scope, i, rowIndex, k);
                                    return true;
                                }
                            }
                        }
                    }
            }
            return false;
        },
        newDataSheetRow: function () {
            var self = this;
            setTimeout(function () {
                self.executeCommand({ commandName: 'New', commandArgument: self.get_viewId() });
            }, 100);
        },
        editDataSheetRow: function (rowIndex) {
            if (this.get_isDataSheet())
                this.executeRowCommand(rowIndex, 'Edit', '', false);
        },
        deleteDataSheetRow: function () {
            var fc = this._get_focusedCell();
            if (fc) {
                this.executeRowCommand(fc.rowIndex, 'Select');
                var self = this;
                setTimeout(function () {
                    self.executeActionInScope(['Row', 'ActionBar'], 'Delete', null, fc.rowIndex);
                }, 100);
            }
        },
        _moveFocusToNextRow: function (fc2, pageSize) {
            var handled = false;
            var originalDataRowIndex = this._get_selectedDataRowIndex(fc2.rowIndex);
            var originalPageOffset = this._pageOffset;
            if (fc2.rowIndex < pageSize - 1)
                fc2.rowIndex++;
            else if (this._get_selectedDataRowIndex(fc2.rowIndex) < this._totalRowCount - 1) {
                if (this._pageOffset == null)
                    this._pageOffset = 1;
                else
                    this._pageOffset++;
                handled = true;
                if (this._pageOffset == pageSize) {
                    this._pageOffset = null;
                    this.goToPage(this.get_pageIndex() + 1);
                }
                else
                    this.goToPage(this.get_pageIndex());
            }
            if (originalDataRowIndex == this._get_selectedDataRowIndex(fc2.rowIndex) && !this.editing() && this._allowNew()) {
                this._ignoreSelectedKey = true;
                this.newDataSheetRow();
                handled = true;
            }
            return handled;
        },
        _scrollToRow: function (delta) {
            return;
            // not implemented
            //var fc = this._get_focusedCell();
            //if (fc) {
            //    fc.rowIndex += delta > 0 ? 1 : -1;
            //    this._moveFocusToNextRow(fc, this.get_pageSize);
            //}
        },
        cancelDataSheetEdit: function () {
            if (this.editing()) {
                var fc = this._get_focusedCell();
                if (fc != null)
                    this.executeRowCommand(fc.rowIndex, 'Cancel', null, false);
                return true;
            }
            else
                return false;
        },
        _inputListenerClick: function (e) {
            if (this._skipClickEvent) {
                this._skipClickEvent = false;
                return;
            }
            if (this._lookupIsActive) return;
            var elem = e.target;
            var isThisContainer = false;
            var isDataCell = false;
            var keepFocus = true;
            while (elem != null) {
                if (elem == this._container) {
                    isThisContainer = true;
                    break;
                }
                if (elem.className != null) {
                    if (elem.className.match(/Cell|Group|InfoRow|FieldHeaderSelector|Toggle|FooterRow|ActionRow\s*/))
                        isDataCell = true;
                    else if (elem.className.match(/QuickFind|SearchBarFrame\s*/))
                        keepFocus = false;
                }

                elem = elem.parentNode;
            }
            if (!isThisContainer)
                this.cancelDataSheet();
            else {
                if (keepFocus)
                    this._lostFocus = !isDataCell;
                else
                    this._lostFocus = true;
                this._skipEditOnClick = true;
                if (!isDataCell)
                    this.cancelDataSheetEdit();
            }
        },
        _inputListenerDblClick: function (e) {
            if (this._lostFocus) return;
            var fc = this._get_focusedCell();
            if (!fc || this.editing() || !this._allowEdit()) return;
            //this.executeRowCommand(fc.rowIndex, 'Edit', this.get_viewId(), false);
            if (document.selection)
                document.selection.clear();
            this.editDataSheetRow(fc.rowIndex);
        },
        _get_focusedCell: function () {
            return this._focusedCell;
        },
        _focusCell: function (rowIndex, colIndex, highlight) {
            if (!this.get_isDataSheet()) {
                this._focusedCell = null;
                return null;
            }
            var inserting = this.inserting();
            if (highlight == null)
                highlight = true;
            if (rowIndex == -1 && colIndex == -1) {
                if (!this._focusedCell) return null;
                rowIndex = this._focusedCell.rowIndex;
                colIndex = this._focusedCell.colIndex;
            }
            if (rowIndex >= this._rows.length)
                rowIndex = this._rows.length - 1;
            if (colIndex >= this._fields.length)
                colIndex = this._fields.length - 1;
            var tableRows = this._container.childNodes[0].rows;
            var currentRowIndex = -1;

            for (var i = 0; i < tableRows.length; i++) {
                var row = tableRows[i];
                if (Sys.UI.DomElement.containsCssClass(row, 'Row') || Sys.UI.DomElement.containsCssClass(row, 'AlternatingRow'))
                    currentRowIndex++;
                if (inserting) {
                    if (Sys.UI.DomElement.containsCssClass(row, 'Inserting'))
                        break;
                }
                else if (currentRowIndex == rowIndex)
                    break;
            }
            if (currentRowIndex < 0) return null;
            var currentColIndex = -1;
            for (i = 0; i < row.childNodes.length; i++) {
                var cell = row.childNodes[i + this.get_sysColCount()];
                if (cell && Sys.UI.DomElement.containsCssClass(cell, 'Cell'))
                    currentColIndex++;
                if (currentColIndex == colIndex)
                    break;
            }
            if (currentColIndex < 0) return null;
            var gapCell = cell.parentNode.childNodes[this.get_sysColCount() - 1];
            var headerRow = this._get_headerRowElement();
            var headerCell = headerRow.childNodes[colIndex + this.get_sysColCount()];
            if (highlight == true) {
                var headerCellBounds = $common.getBounds(headerCell);
                Sys.UI.DomElement.addCssClass(cell, 'Focused');
                Sys.UI.DomElement.addCssClass(gapCell, 'CrossHair');
                Sys.UI.DomElement.addCssClass(headerCell, 'CrossHair');
                if (!this._skipCellFocus) {
                    var scrolling = _app.scrolling(); // $common.getScrolling();
                    var clientBounds = $common.getClientBounds();
                    var cellBounds = $common.getBounds(cell);
                    if (scrolling.y > cellBounds.y)
                        (currentRowIndex == 0 ? headerCell : cell).scrollIntoView(true);
                    else if (scrolling.y + clientBounds.height <= cellBounds.y + cellBounds.height)
                        cell.scrollIntoView(false);
                    else if (scrolling.x > cellBounds.x || scrolling.x + clientBounds.width - 1 <= cellBounds.x || scrolling.x + clientBounds.width - 1 <= cellBounds.x + cellBounds.width)
                        cell.scrollIntoView(false);
                    if (Sys.Browser.agent == Sys.Browser.InternetExplorer/* && this.editing()*/) {
                        var rb = $common.getBounds(headerRow);
                        headerRow.style.height = rb.height + 'px';
                    }
                }
                var headerCellBounds2 = $common.getBounds(headerCell);
                if (headerCellBounds.width != headerCellBounds2.width || headerCellBounds.x != headerCellBounds2.x || headerCellBounds.y != headerCellBounds2.y)
                    Sys.UI.DomElement.addCssClass(headerCell, 'Narrow');
                this._skipCellFocus = false;
            }
            else {
                Sys.UI.DomElement.removeCssClass(cell, 'Focused');
                Sys.UI.DomElement.removeCssClass(cell, 'Narrow');
                Sys.UI.DomElement.removeCssClass(gapCell, 'CrossHair');
                Sys.UI.DomElement.removeCssClass(headerCell, 'CrossHair');
            }
            this._focusedCell = { 'rowIndex': rowIndex, 'colIndex': colIndex }
            return cell;
        },
        _initializeModalPopup: function () {
            Sys.UI.DomElement.addCssClass(this.get_element(), 'ModalPlaceholder');
            var cb = $common.getClientBounds();
            var width = cb.width / 5 * 4;
            var maxWidth = resourcesModalPopup.MaxWidth;
            var confirmContext = this.get_confirmContext();
            if (confirmContext && confirmContext.MaxWidth > 0 && confirmContext.MaxWidth < width)
                maxWidth = confirmContext.MaxWidth;
            if (width > maxWidth)
                width = maxWidth;
            var height = cb.height / 5 * 4;
            if (this._container.style.overflowX != null) {
                this._container.style.overflowY = 'auto';
                this._container.style.overflowX = 'hidden';
            }
            else
                this._container.style.overflow = 'auto';
            this._container.style.height = height + 'px';
            this._container.style.width = width + 'px';
            this._saveTabIndexes();
            this._modalPopup = $create(AjaxControlToolkit.ModalPopupBehavior, { id: this.get_id() + 'ModalPopup' + Sys.Application.getComponents().length, PopupControlID: this.get_element().id, DropShadow: true, BackgroundCssClass: 'ModalBackground' }, null, null, this.get_modalAnchor());
            this._modalPopup.show();
        },
        _resizeContainerBounds: function () {
            if (!this._modalPopup)
                this._container.style.height = '';
            var containerBounds = $common.getBounds(this._container);
            var clientBounds = $common.getClientBounds();
            var maxHeight = Math.ceil(clientBounds.height / 5 * 4);
            if (containerBounds.height > maxHeight) {
                this._container.style.height = maxHeight + 'px';
                containerBounds.skipResizing = true;
            }
            return containerBounds;
        },
        _adjustModalPopupSize: function () {
            var confirmContext = this.get_confirmContext();
            Sys.UI.DomElement.removeCssClass(this._element, 'EmptyModalDialog');
            var sb = new Sys.StringBuilder();
            var rowsToDelete = [];
            var tables = this._container.getElementsByTagName('table');
            for (var i = tables.length - 1; i >= 0; i--) {
                var t = tables[i];
                if (t.className == 'ActionButtons') {
                    if (sb.isEmpty()) {
                        sb.append('<table class="DataView" cellSpacing=0 cellPadding=0><tr class="ActionButtonsRow BottomButtonsRow">')
                        sb.append(t.parentNode.parentNode.innerHTML);
                        sb.append('</tr></table>');
                    }
                    Array.add(rowsToDelete, t.parentNode.parentNode);
                }
            }
            while (rowsToDelete.length > 0) {
                rowsToDelete[0].parentNode.removeChild(rowsToDelete[0]);
                delete rowsToDelete[0];
                Array.removeAt(rowsToDelete, 0);
            }
            var contentElem = this._container.childNodes[0];
            contentElem.style.width = '';
            contentElem.style.height = '';
            var contentSize = $common.getContentSize(contentElem);
            contentSize.height += Sys.Browser.agent === Sys.Browser.InternetExplorer && Sys.Browser.version < 8 ? 3 : 1;
            if (!this._buttons) {
                this._buttons = document.createElement('div');
                this.get_element().appendChild(this._buttons);
                this._buttons.style.width = contentSize.width + 'px';
                Sys.UI.DomElement.addCssClass(this._buttons, 'FixedButtons');
                this._title = document.createElement('div');
                //this._title.innerHTML = _app.htmlEncode(this.get_view().Label);
                Sys.UI.DomElement.addCssClass(this._title, 'FixedTitle');
                this.get_element().insertBefore(this._title, this._container);
                if (!__designer())
                    $(this._element).draggable({
                        'handle': this._title, drag: function (event, ui) {
                            var w = $window;
                            var clientWidth = w.width();
                            var clientHeight = w.height();
                            var left = ui.offset.left - w.scrollLeft();
                            var top = ui.offset.top - w.scrollTop();
                            var width = ui.helper.outerWidth();
                            if (left + width < 50 || top < 5 || left > clientWidth - 50 || top > clientHeight - 75)
                                return false;
                        }
                    });
            }
            else {
                if (!this._modalAutoSized) {
                    //this._buttons.style.width = 'auto';
                    //this._title.style.width = 'auto';
                    this._container.style.width = 'auto';
                    this._modalAutoSized = true;
                }
            }
            this._buttons.innerHTML = sb.toString();
            sb.clear();
            //        var containerBounds = $common.getBounds(this._container);
            //        var clientBounds = $common.getClientBounds();
            //        var maxHeight = Math.ceil(clientBounds.height / 5 * 4);
            //        if (containerBounds.height > maxHeight)
            //            this._container.style.height = maxHeight + 'px';
            var containerBounds = this._resizeContainerBounds();
            if (containerBounds.height > contentSize.height && !containerBounds.skipResizing) {
                var cbb = $common.getBorderBox(contentElem);
                contentSize.width += cbb.horizontal;
                $common.setContentSize(this._container, contentSize);
            }
            contentElem.style.width = this._title.offsetWidth + 'px';
            this._buttons.style.width = this._title.offsetWidth + 'px';
            Sys.UI.DomElement.setVisible(this.get_element(), true);
            //        if (this._modalPopup) {
            //            if (Sys.Browser.agent === Sys.Browser.InternetExplorer) this._modalPopup.hide();
            //            this._modalPopup.show();
            //        }
            var b = $common.getBounds(this._container);
            var tb = $common.getPaddingBox(this._title);
            var bb = $common.getBorderBox(this._title);
            this._title.style.width = (b.width - tb.horizontal - bb.horizontal) + 'px';
            tb = $common.getPaddingBox(this._buttons);
            bb = $common.getBorderBox(this._buttons);
            this._buttons.style.width = (b.width - tb.horizontal - bb.horizontal) + 'px';
            this._title.innerHTML = String.format('<table style="width:100%" cellpadding="0" cellspacing="0"><tr><td><div class="Text">{1}</div></td><td align="right"><a href="javascript:" class="Close" onclick="$find(\'{0}\').endModalState(\'Cancel\');return false" tabindex="{3}" title="{2}">&nbsp;</a></td></tr></table>',
                this.get_id(),
                confirmContext && confirmContext.WindowTitle ? confirmContext.WindowTitle : _app.htmlEncode(this.get_view().Label),
                resourcesModalPopup.Close,
                $nextTabIndex());
            //if (Sys.Browser.agent === Sys.Browser.InternetExplorer && this.editing()) this._focus();
            this._modalPopup.show();
            if (this._modalAutoSized && !this._modalWidthFixed) {
                this._modalWidthFixed = true;
                this._container.style.width = this._container.offsetWidth + 'px';
            }
            this._adjustModalHeight(true);
        },
        _adjustModalHeight: function (save) {
            var container = this._container;
            if (this._modalPopup && this.get_viewType() == 'Form')
                if (save) {
                    //var containerBounds = $common.getBounds(container);
                    //this._lastContainerHeight = containerBounds.height;
                    this._lastContainerHeight = $(container).height();
                }
                else if (this._lastContainerHeight != null) {
                    var oldStyleHeight = container.style.height;
                    var oldScrollTop = container.scrollTop;
                    container.style.height = '';
                    var categoriesCell = null;
                    var trList = container.getElementsByTagName('tr');
                    for (var i = 0; i < trList.length; i++) {
                        if (trList[i].className == 'Categories') {
                            categoriesCell = trList[i].childNodes[0];
                            break;
                        }
                    }
                    if (categoriesCell)
                        categoriesCell.style.paddingBottom = '';
                    containerBounds = $common.getBounds(container);
                    if (this._lastContainerHeight > containerBounds.height) {
                        if (categoriesCell) {
                            var paddingBox = $common.getPaddingBox(categoriesCell);
                            var newPaddingBottom = this._lastContainerHeight - containerBounds.height - paddingBox.bottom;
                            if (Sys.Browser.agent == Sys.Browser.InternetExplorer)
                                newPaddingBottom++;
                            categoriesCell.style.paddingBottom = newPaddingBottom + 'px';
                        }
                    }
                    else {
                        var clientBounds = $common.getClientBounds();
                        if (clientBounds.height * 0.8 <= containerBounds.height) {
                            container.style.height = oldStyleHeight;
                            container.scrollTop = oldScrollTop;
                        }
                        else
                            this._lastContainerHeight = containerBounds.height;
                    }
                }
        },
        _allowModalAutoSize: function () {
            this._modalWidthFixed = false;
            this._modalAutoSized = false;
        },
        _disposeModalPopup: function () {
            if (!this._modalPopup) return;
            this._modalPopup.hide();
            this._modalPopup.dispose();
            //delete this._modalPopup._backgroundElement;
            //delete this._modalPopup._foregroundElement;
            //delete this._modalPopup._popupElement;
            if (!__designer())
                $(this._element).draggable('destroy');
            delete this._buttons;
            delete this._title;
            delete this._modalAnchor;
            var elem = this.get_element();
            elem.parentNode.removeChild(elem);
            this._restoreTabIndexes();
        },
        endModalState: function (commandName) {
            var parentDataView = this.get_parentDataView();
            function refreshParentDataView() {
                parentDataView.refresh();
            }
            if (this.get_isModal()) {
                var exitCommands = this.get_exitModalStateCommands();
                if (exitCommands) {
                    for (var i = 0; i < exitCommands.length; i++) {
                        if (commandName == exitCommands[i]) {
                            if (parentDataView) {
                                parentDataView._lookupIsActive = false;
                                parentDataView._skipClickEvent = true;
                            }
                            if (_touch)
                                _touch.endModalState(parentDataView, this);
                            else
                                this.dispose();
                            return true;
                        }
                    }
                }
            }
            if (parentDataView && !this.get_confirmContext())
                if (!_touch)
                    parentDataView.refresh();
            closeHoverMonitorInstance();
            return false;
        },
        _adjustLookupSize: function () {
            //if (this.get_lookupField() && _app.isIE6) this.get_lookupField()._lookupModalBehavior._layout();
            if (this.get_lookupField() && this.get_pageSize() > 3) {
                var scrolling = _app.scrolling(); // $common.getScrolling();
                var clientBounds = $common.getClientBounds()
                var b = $common.getBounds(this.get_element());
                if (b.height + b.y > clientBounds.height + scrolling.y)
                    this.set_pageSize(Math.ceil(this.get_pageSize() * 0.66));
            }
        },
        _onMethodFailed: function (err, response, context) {
            this._busy(false);
            if (_app._navigated) return;
            //if (this._wsRequest == null) return;
            _app.showMessage(String.format('<pre style="word-wrap:break-word;margin:0px">Component: {4}\r\nController: {5}; View: {6}; Timed out: {0}; Status Code: {7};\r\nException: {1}\r\nMessage: {2}\r\nStack:\r\n{3}</pre>',
                err.get_timedOut(), err.get_exceptionType(), err.get_message(), err.get_stackTrace(), this.get_id(), this.get_controller(), this.get_viewId(), err.get_statusCode()));
            $(this.get_element()).css('border', '1px red solid');
        },
        _createArgsForListOfValues: function (distinctFieldName) {
            var lc = this.get_lookupContext(),
                filter = this._searchBarVisibleIndex == null ? this.get_filter() : this._createSearchBarFilter(true),
                request = {
                    FieldName: distinctFieldName, Controller: this.get_controller(), View: this.get_viewId(),
                    Filter: this._combinedFilter(filter.length == 1 && filter[0].match(/(\w+):/)[1] == distinctFieldName ? null : filter),
                    ExternalFilter: this.get_externalFilter(),
                    LookupContextFieldName: lc ? lc.FieldName : null, LookupContextController: lc ? lc.Controller : null,
                    LookupContextView: lc ? lc.View : null, Tag: this.get_tag(),
                    QuickFindHint: _touch ? this.viewProp('quickFindHint') : null
                };
            this._useSearchParams(request);
            return { controller: this.get_controller(), view: this.get_viewId(), request: request };
        },
        _loadListOfValues: function (family, fieldName, distinctFieldName, callback) {
            this._busy(true);
            //var lc = this.get_lookupContext();
            //var filter = this.get_filter();
            this._invoke('GetListOfValues', this._createArgsForListOfValues(distinctFieldName), //{ controller: this.get_controller(), view: this.get_viewId(), request: { FieldName: distinctFieldName, Filter: filter.length == 1 && filter[0].match(/(\w+):/)[1] == distinctFieldName ? null : filter, LookupContextFieldName: lc ? lc.FieldName : null, LookupContextController: lc ? lc.Controller : null, LookupContextView: lc ? lc.View : null} },
                Function.createDelegate(this, this._onGetListOfValuesComplete), { 'family': family, 'fieldName': fieldName, callback: callback });
        },
        _onGetListOfValuesComplete: function (result, context) {
            this._busy(false);
            var field = this.findField(context.fieldName);
            field._listOfValues = result;
            if (result[result.length - 1] == null) {
                Array.insert(result, 0, result[result.length - 1]);
                Array.removeAt(result, result.length - 1);
            }
            if (context.callback)
                context.callback();
            else {
                if (this.get_isChart()) {
                    if (this.get_showViewSelector())
                        this._registerViewSelectorItems();
                    else
                        this._registerActionBarItems();
                    _web.HoverMonitor._instance._tempOpenDelay = 100;
                }
                else
                    this._registerFieldHeaderItems(Array.indexOf(this.get_fields(), field));
                $refreshHoverMenu(context.family);
                _app._resized = true;
            }
        },
        get_parentDataView: function (defaultParent) {
            var dataView = defaultParent;
            if (this._parentDataViewId)
                dataView = findDataView(this._parentDataViewId);
            return dataView;
        },
        get_selectedValues: function () {
            var dataView = this.get_parentDataView(this); // this;
            //if (!isNullOrEmpty(this._parentDataViewId))
            //    dataView = $find(this._parentDataViewId);
            var selection = dataView ? dataView.get_selectedValue() : [],
                selectionLength, nullIndex;
            selection = selection.length == 0 ? [] : (!dataView.multiSelect() ? [selection] : selection.split(';'));
            nullIndex = Array.indexOf(selection, 'null');
            if (nullIndex != -1)
                selection.splice(nullIndex, 1);
            return selection;
        },
        _execute: function (args) {
            this._busy(true);
            this._showWait();
            this._lastArgs = args;
            /*
            args.Filter = this.get_filter();
            args.SortExpression = this.get_sortExpression();
            args.SelectedValues = this.get_selectedValues();
            args.ExternalFilter = this.get_externalFilter();
            args.Transaction = this.get_transaction();
            if (!isNullOrEmpty(args.Transaction) && !this.get_isModal() && !this.get_filterSource() && args.CommandName.match(/Insert|Update|Delete/))
            args.Transaction += ':complete';
            args.SaveLEVs = this._allowLEVs == true;
            */
            this._invoke('Execute', { controller: args.Controller, view: args.View, args: args }, Function.createDelegate(this, this._onExecuteComplete));
        },
        _populateDynamicLookups: function (result) {
            var that = this, i, lov;
            for (i = 0; i < result.Values.length; i++) {
                var v = result.Values[i],
                    f = that.findField(v.Name);
                if (f) {
                    lov = f.DynamicItems = v.NewValue;
                    if (f.requiresDynamicNullItem && f.ItemsStyle.match(/^(ListBox|RadioButtonList|DropDownList)$/) && !f.ItemsTargetController)
                        lov.splice(0, 0, [null, f.is('lookup-any-value') ? resourcesData.AnyValue : resourcesData.NullValueInForms]);
                    f.Items = [];
                    f.ItemCache = null;
                }
            }
            if (_touch)
                //this.extension().afterPopulateDynamicLookups(result.Values);
                _app.input.execute({ dataView: that, values: result.Values, populateDynamicLookups: false });
            else {
                // desktop style of refreshing
                that._skipPopulateDynamicLookups = true;
                that.refresh(true);
                that._focus();
            }
        },
        _updateCalculatedFields: function (result) {
            this._displayActionErrors(result);
            var values = []
            for (var i = 0; i < result.Values.length; i++) {
                var v = result.Values[i];
                Array.add(values, { 'name': v.Name, 'value': v.NewValue });
            }
            this.refresh(true, values);
            //this._focus(values.length > 0 ? values[values.length - 1].name : null);
            this._focus();
        },
        _get_LEVs: function () {
            for (var i = 0; i < _app.LEVs.length; i++) {
                var lev = _app.LEVs[i];
                if (lev.controller == this.get_controller())
                    return lev.records;
            }
            lev = { 'controller': this.get_controller(), 'records': [] };
            Array.add(_app.LEVs, lev);
            return lev.records;
        },
        _recordLEVs: function (values) {
            if (!this._allowLEVs || !values && !this._lastArgs.CommandName.match(/Insert|Update/)) return;
            if (!(this._lastArgs || values)) return;
            if (!values) values = this._lastArgs.Values;
            var levs = this._get_LEVs();
            var skip = true;
            for (var i = 0; i < values.length; i++) {
                if (values[i].Modified) {
                    skip = false;
                    break;
                }
            }
            if (skip) return;
            if (levs.length > 0)
                Array.removeAt(levs, levs.length - 1);
            Array.insert(levs, 0, values)
        },
        _applyLEV: function (fieldIndex) {
            var f = this._allFields[fieldIndex];
            var f2 = this._allFields[f.AliasIndex];
            var values = [];
            var r = this._get_LEVs()[0];
            for (var i = 0; i < r.length; i++) {
                var v = r[i];
                if (v.Name == f.Name || v.Name == f2.Name)
                    Array.add(values, { 'name': v.Name, 'value': v.NewValue });
            }
            this.refresh(true, values);
        },
        _notifyMaster: function () {
            var that = this, dv, field, fieldName;
            if (that.get_hasParent())
                if (_touch) {
                    dv = that;
                    fieldName = dv._dataViewFieldName
                    if (!(dv._filterSource && fieldName))
                        while (dv && !fieldName) {
                            dv = dv.get_parentDataView();
                            if (dv)
                                fieldName = dv._dataViewFieldName;
                        }
                    if (dv && fieldName) {
                        dv = findDataView(dv._filterSource);
                        field = dv.findField(fieldName);
                        if (field && field.CausesCalculate)
                            if (dv == _touch.dataView())
                                dv._raiseCalculate(field, field);
                            else
                                _touch.pageInfo(dv).calculate = fieldName;

                    }
                }
                else {
                    var m = findDataView(that.get_filterSource());
                    if (m) m._updateDynamicValues('controller:' + that.get_controller());
                }
        },
        _clearSelectedKey: function (delaySelected) {
            // this workaround compensates for "modal" lookups in mobile client.
            var dataView = this._lookupInfo ? this : this.get_parentDataView(this),
                extension,
                key = dataView._selectedKey,
                keyIndex;
            if (key && key.length) {
                keyIndex = dataView._selectedKeyList.indexOf(key.join(';'));
                if (keyIndex != -1)
                    dataView._selectedKeyList.splice(keyIndex, 1);
            }
            dataView._selectedKey = [];
            dataView._selectedKeyFilter = [];
            dataView._selectedRowIndex = null;
            dataView._position = null;
            if (delaySelected != false)
                dataView._raiseSelectedDelayed = true;
            dataView._selectedRow = null;
        },
        _refreshSelectedKey: function (values, notify) {
            var that = this,
                dataView, extension,
                selectedKey = [],
                selectedKeyFilter = [],
                field,
                i, j, v;
            if (values.length == 0 && that._lastArgs)
                values = that._lastArgs.Values;
            for (i = 0; i < that._keyFields.length; i++) {
                field = that._keyFields[i];
                for (j = 0; j < values.length; j++) {
                    if (values[j].Name == field.Name) {
                        v = values[j];
                        if (v.NewValue == null) {
                            that._clearSelectedKey();
                            return;
                        }
                        Array.add(selectedKey, v.NewValue);
                        Array.add(selectedKeyFilter, field.Name + ':=' + that.convertFieldValueToString(field, v.NewValue));
                        break;
                    }
                }
            }
            dataView = that.get_parentDataView(that);
            if (selectedKey.length > 0) {
                dataView._selectedKey = selectedKey;
                dataView._selectedKeyFilter = selectedKeyFilter;
                dataView._selectedKeyList = [selectedKey.join(';')];
                dataView._raiseSelectedDelayed = true;
            }
            else
                dataView._selectedKeyList = [];
        },
        _cancelConfirmation: function () {
            if (this._confirmDataViewId) {
                var confirmView = findDataView(this._confirmDataViewId);
                if (confirmView) {
                    this._confirmDataViewId = null;
                    confirmView.cancel();
                }
            }
        },
        _findKeyValue: function (list) {
            var keyField = this._keyFields[0];
            for (var i = 0; i < list.length; i++) {
                var fv = list[i];
                if (keyField.Name == fv.Name)
                    return fv.NewValue;
            }
            return null;
        },
        _onExecuteComplete: function (result, context) {
            var that = this,
                extension = that.extension(),
                lastArgs = that._lastArgs,
                lastCommandName = lastArgs.CommandName,
                resultValues = result.Values,
                notification;
            that._busy(false);
            that._hideWait();

            if (_touch)
                notification = _touch.notify(that);

            var noErrors = result.Errors.length == 0,
                pendingUploads = that._pendingUploads;
            if (pendingUploads && pendingUploads.length > 0 && noErrors && lastCommandName.match(/^(Insert|Update)$/)/* && result.RowsAffected == 1*/) {
                // select the first pending upload
                var upload = pendingUploads[0],
                    key,
                    blobField;
                Array.removeAt(pendingUploads, 0);
                // update the key reference in the pending upload
                key = that._findKeyValue(resultValues);
                if (key == null)
                    key = that._findKeyValue(lastArgs.Values);
                if (upload.files) {
                    blobField = that.findField(upload.fieldName);
                    if (blobField.Name.match(/_Annotation_Attachment/))
                        key = blobField._dataView._controller + ',' + blobField.Name + '|' + key;
                    _app.upload('execute', {
                        container: findBlobContainer(that, blobField), // $(_touch ? _app.touch.page(that._id) : that._container).find('.drop-box-' + blobField.Index),
                        files: upload.files,
                        url: String.format('{0}Blob.ashx?{1}=u|{2}&_v=2', __baseUrl, blobField.OnDemandHandler, key),
                        success: function () {
                            that._stopWaiting = true;
                        },
                        error: function () {
                            pendingUploads.push(upload);
                            that._stopWaiting = true;
                        }
                    });
                }
                else
                    upload.form.action = upload.form.action.replace(/=u\|&/, String.format('=u|{0}&', key));
                // start the pending upload
                that._pendingExecuteComplete = { result: result, context: context };
                that._showDownloadProgress();
                if (upload.form)
                    upload.form.submit();
                return;
            }
            var lastFocusedCell = that._lastFocusedCell;
            if (lastCommandName == 'PopulateDynamicLookups') {
                that._populateDynamicLookups(result);
                return;
            }
            else if (lastCommandName == 'Calculate') {
                if (_touch)
                    //if (that.get_view().Layout)
                    _app.input.execute({ dataView: that, values: resultValues, raiseCalculate: false });
                //else
                //    extension.afterCalculate(resultValues);
                else
                    that._updateCalculatedFields(result);
                if (result.ClientScript)
                    eval(result.ClientScript);
                if (that._pendingPopulate) {
                    that._pendingPopulate = false;
                    that._raisePopulateDynamicLookups();
                }
                return;
            }
            var stopFlow = false;
            var isCustom = lastCommandName == 'Custom';
            if (noErrors)
                if (_touch) {
                    var syncMap = _app.controllerSyncMap[that._controller];
                    if (syncMap)
                        for (var key in syncMap)
                            syncMap[key] = true;
                }
            if (that._confirmDataViewId && _touch) {
                    /*$(document).one('pagecontainershow', */_touch.pageShown(function () {
                    that._onExecuteComplete(result, context);
                });
                that._cancelConfirmation();
                return;
            }
            else
                that._cancelConfirmation();
            var ev = { 'result': result, 'context': context, 'handled': false }
            that.raiseExecuted(ev);
            if (ev.handled) return;
            if (result.ClearSelection) {
                that._clearSelectedKey();
                that._selectedKeyList = [];
            }
            var existingRow = !lastCommandName.match(/Insert/);
            if (lastCommandName.match(/Delete/i) && result.RowsAffected > 0) {
                extension = that.get_parentDataView(that).extension();
                if (extension && extension.clearSelection)
                    extension.clearSelection(true);
                else
                    that._clearSelectedKey();
            }
            else
                if (lastCommandName.match(/Custom|SQL|Email/)) {
                    stopFlow = !isCustom && that.editing();
                    if (resultValues.length > 0) {
                        if (_touch)
                            //if (that.get_view().Layout)
                            _app.input.execute({ dataView: that, values: resultValues });
                        //else
                        //    extension.afterCalculate(resultValues);
                        else
                            that.refresh(true, resultValues)
                        if (isCustom && !result.ClientScript)
                            result.ClientScript = 'void(0)';
                        that._refreshSelectedKey(resultValues, noErrors);
                    }
                    else if (!stopFlow && lastCommandName == 'SQL') {
                        that._raiseSelectedDelayed = true;
                        that._forceChanged = true;
                    }
                }
                else
                    if (existingRow) {
                        for (var i = 0; i < resultValues.length; i++) {
                            var v = resultValues[i];
                            var field = that.findField(v.Name);
                            if (field) that.get_currentRow()[field.Index] = v.NewValue;
                        }
                    }
                    else {
                        if (_touch) {
                            _app.input.execute({ dataView: that, values: resultValues });
                            if (that.get_isForm()) {
                                // the parent data view does not have the command row yet - let's create one
                                var parentDataView = that.get_parentDataView(null),
                                    parentCommandRow, editRow;
                                if (parentDataView && parentDataView != that) {
                                    parentCommandRow = [];
                                    editRow = that.editRow();
                                    $(that._allFields).each(function () {
                                        var f = this,
                                            pf = parentDataView.findField(f.Name),
                                            v = editRow[f.Index];
                                        if (pf)
                                            parentCommandRow[pf.Index] = v;
                                    });
                                    parentDataView.extension()._commandRow = parentCommandRow;
                                }
                            }
                        }
                        that._refreshSelectedKey(resultValues, noErrors);
                    }
            if (noErrors) {
                that._forceSync();
                var parentDataView = that.get_parentDataView();
                if (parentDataView)
                    parentDataView._forceSync();
                that.tag(result.Tag);
                that._requestedFilter = result.Filter;
                that._requestedSortExpression = result.SortExpression;
                that._autoRefresh();
                if (that.multiSelect()) {
                    if (result.KeepSelection)
                        that._keepKeyList = true;
                    else {
                        that._selectedKeyList = [];
                        that.set_selectedValue('');
                    }
                }
                that._recordLEVs();
                that.updateSummary();
                that._continueAfterScript = true;
                if (result.ClientScript) {
                    that._continueAfterScript = false;
                    result.ClientScript = that.resolveClientUrl(result.ClientScript);
                    eval(result.ClientScript);
                }
                that._lastFocusedCell = null;
                var lastCommandArgument = lastArgs.CommandArgument;
                var rules = new _businessRules(that);
                rules.after({ commandName: lastCommandName, commandArgument: lastCommandArgument });
                if (rules.canceled())
                    that._continueAfterScript = false;
                rules.dispose();
                if (that._continueAfterScript) {
                    if (result.NavigateUrl) {
                        result.NavigateUrl = that.resolveClientUrl(result.NavigateUrl);
                        that.navigate(result.NavigateUrl, existingRow ? lastArgs.Values : resultValues);
                    }
                    else {
                        if (that._closeInstantDetails()) { }
                        else if (that.endModalState(lastCommandName)) { }
                        else if (that.get_backOnCancel() || !isNullOrEmpty(that._replaceTriggerViewId)) that.goBack(true);
                        else {
                            that._notifyDesigner(true);
                            if (notification != null)
                                _touch.notify({ text: notification, force: true });
                            var actions = that.get_actions(that._get_lastActionScope(), true),
                                a, dataView,
                                lastArgsValues = that._lastArgs.Values,
                                commandName, commandArgument;
                            for (i = 0; i < actions.length; i++) {
                                a = actions[i];
                                if (a.WhenLastCommandName == lastCommandName && (!a.WhenLastCommandArgument || a.WhenLastCommandArgument == lastCommandArgument) && that._isActionMatched(a)) {
                                    commandName = a.CommandName;
                                    commandArgument = a.CommandArgument;
                                    if (_touch)
                                        dataView = that.delegateCommand(commandName, commandArgument);
                                    if (!dataView)
                                        dataView = that;
                                    //executeNextAction();
                                    if (!a.WhenKeySelected || (dataView.get_selectedKey() || []).length) {
                                        dataView.set_lastCommandName(lastCommandName);
                                        dataView.set_lastCommandArgument(lastCommandArgument);
                                        dataView.executeCommand({ commandName: commandName, commandArgument: commandArgument, path: a.Path, causesValidation: a.CausesValidation, values: lastCommandName.match(/Insert|Custom/) ? lastArgsValues : null });
                                        dataView._pendingSelectedEvent = commandName.match(/^(Edit|Select)/) != null;
                                        dataView._notifyMaster();
                                    }
                                    return;
                                }
                            }
                            if (!stopFlow) {
                                that._pendingSelectedEvent = lastCommandName.match(/Update|Custom|SQL/);
                                that.set_lastCommandName(null);
                                if (that.get_isModal() && (!that.get_isForm(that._lastViewId) || that._inlineEditor || that._closeViewDetails))
                                    that.endModalState('Cancel');
                                else {
                                    if (_touch && that._viewId == that._lastViewId)
                                        _touch.syncWithOdp(_touch.dataView());
                                    if (_touch && that.get_isForm()) {
                                        if (that._doneCallback)
                                            that._doneCallback(that);
                                        else
                                            location.replace(location.href);
                                    }
                                    else
                                        that.goToView(that._lastViewId);

                                }
                            }
                        }
                    }
                    that._notifyMaster();
                }
                else if (_touch)
                    _touch.edit.sync({ reset: true });

            }
            else {
                if (lastFocusedCell) {
                    that._focusCell(-1, -1, false);
                    that._focusCell(lastFocusedCell.rowIndex, lastFocusedCell.colIndex, true);
                    that._lastFocusedCell = null;
                    that._saveAndNew = false;
                }
                if (result.ClientScript) {
                    result.ClientScript = that.resolveClientUrl(result.ClientScript);
                    eval(result.ClientScript);
                }
                that._displayActionErrors(result);
            }
        },
        _get_lastActionScope: function () {
            var path = this._lastArgs ? this._lastArgs.Path : null;
            if (path) {
                var m = path.match(/^(\w+)\//);
                if (m) {
                    var list = this._actionGroups;
                    for (var i = 0; i < list.length; i++) {
                        var ag = list[i];
                        if (ag.Id == m[1])
                            return ag.Scope;
                    }
                }
            }
            var viewType = this.get_viewType();
            if (viewType.match(/DataSheet|Tree/))
                viewType = 'Grid';
            return viewType;
        },
        _displayActionErrors: function (result) {
            if (result.Errors.length == 0) return;
            var sb = new Sys.StringBuilder(), i;
            for (i = 0; i < result.Errors.length; i++)
                sb.append((i ? '\n' : '') + _app.formatMessage('Attention', result.Errors[i]));
            if (_touch)
                _touch.edit.sync({ reset: true });
            _app.showMessage(sb.toString());
            sb.clear();
        },
        _busy: function (isBusy) {
            var that = this,
                api = that._api;
            if (isBusy == null)
                return that._isBusy;
            if (api && api.background)
                return;
            that._isBusy = isBusy;
            that._enableButtons(!isBusy);
            //if (_app.isSaaS()) return;
            var interval = that._busyInterval,
                atWindowTop = !_touch && (that.get_isModal() || that.get_lookupField() != null || !this._controller) || _touch != null || this.get_useCase() == '$app',
                containerElement = _touch ? _touch.toolbar() : (atWindowTop ? document.body : that._container),
                $container = $(containerElement),
                $viewContainer = $(that._container),
                busyIndicator;
            if (_touch && !_touch.toolbar().is(':visible')) {
                _touch.wait(isBusy);
                return;
            }
            if (_touch) {
                if (arguments.length)
                    _touch.busy(isBusy);
                clearTimeout(_app._busyIndicatorTimeout);
                busyIndicator = _app._busyIndicator;
                if (!busyIndicator) {
                    busyIndicator = _app._busyIndicator = $('<div class="dataview-busy-indicator"></div>').appendTo(document.body);
                    _app._busyCount = 0;
                }

                if (isBusy) {
                    if (!_app._busyCount) {
                        _app._busyTime = +new Date();
                        busyIndicator.removeClass('dataview-busy-indicator-done').css({ top: 0/* $container.is('#app-bar-toolbar') ? 0 : $container.outerHeight() - busyIndicator.outerHeight() - 1*/, 'animation-duration': Math.max(1000, $window.width()) / 1000 * 3000 + 'ms' });
                        busyIndicator.addClass('dataview-busy-indicator-animate');
                    }
                    _app._busyCount++;
                }
                else if (_app._busyCount) {
                    if (_app._busyCount == 1) {
                        if (+new Date() - _app._busyTime > 500) {
                            busyIndicator.toggleClass('dataview-busy-indicator-animate dataview-busy-indicator-done');
                            setTimeout(function () {
                                busyIndicator.removeClass('dataview-busy-indicator-done');
                            }, 1550);
                        }
                        else
                            busyIndicator.removeClass('dataview-busy-indicator-animate dataview-busy-indicator-done');
                    }
                    _app._busyCount--;
                }
                return;
            }

            if (isBusy && !interval) {
                if (!_touch && !$viewContainer.is('.dataview-loaded'))
                    $viewContainer.addClass('dataview-loading');
                if (!atWindowTop) {
                    //var innerContainer = $container.find('.HeaderRow');
                    //if (innerContainer.length == 0)
                    //    innerContainer = $container.find('.ActionRow').next();
                    //if (innerContainer && innerContainer.length > 0)
                    //    $container = innerContainer;
                }
                var w = $container.outerWidth() / 5;
                if (w > 200)
                    w = 200;
                var $busy = that._$busy = $(String.format('<div class="dataview-busy-indicator{0}"></div>', atWindowTop ? ' modal' : ''))
                    .css({ position: 'absolute', width: w, 'z-index': atWindowTop ? (_touch ? 5000 : 3000000) : 0 }),
                    x = -w,
                    h = atWindowTop ? 0 : $busy.height();
                $busy.hide().appendTo(document.body);
                var counter = 0,
                    fullyExtended = false,
                    doShow, isHidden;

                this._busyInterval = setInterval(function () {
                    if (counter != null)
                        if (counter++ > (_touch ? 35 : 75)) {
                            counter = null;
                            doShow = true;
                        }
                        else
                            return;
                    x += Math.ceil(w / 30);
                    if (doShow) {
                        if (!atWindowTop || !_app._busyIndicator) {
                            _app._busyIndicator = true;
                            $busy.show();
                        }
                        else {
                            isHidden = true;
                            $busy.hide();
                        }
                        doShow = false;
                    }
                    else if (isHidden && !_app._busyIndicator) {
                        isHidden = false;
                        _app._busyIndicator = true;
                        $busy.show();
                    }
                    if (x >= 0) {
                        var containerWidth = $container.outerWidth();
                        if (x >= (containerWidth - w) && x < containerWidth)
                            $busy.width(containerWidth - x);
                        else if (x >= (containerWidth)) {
                            x = -w;
                        }
                        if (!isHidden) {
                            var scrollTop = $container.scrollTop();
                            $busy.position({ my: 'left top', at: String.format('left+{0} top{1}{2}', x, atWindowTop ? '+' : '-', atWindowTop ? $window.scrollTop() : h), of: $container });
                        }
                    }
                    else {
                        if (!isHidden) {
                            $busy.width(w + x);
                            $busy.position({ my: 'left top', at: String.format('left top{0}{1}', atWindowTop ? '+' : '-', atWindowTop ? $window.scrollTop() : h), of: $container });
                        }
                    }
                }, 1000 / 60);
            }
            else if (interval) {
                if (!_touch && !$viewContainer.is('.dataview-loaded'))
                    $viewContainer.removeClass('dataview-loading').addClass('dataview-loaded');
                clearInterval(interval);
                that._busyInterval = null;
                $busy = that._$busy;
                if ($busy) {
                    var busyOffset = $busy.offset();
                    var containerOffset = $container.offset();
                    that._$busy = null;
                    $busy.width($container.outerWidth() - (busyOffset.left - containerOffset.left));
                    setTimeout(function () {
                        $busy.remove();
                        if (atWindowTop)
                            _app._busyIndicator = false;
                    }, 500);
                }
            }
        },
        _enableButtons: function (enable) {
            //var buttons = this._element.getElementsByTagName('button');
            //for (var i = 0; i < buttons.length; i++) {
            //    var b = buttons[i];
            //    if (b)
            //        if (!enable) {
            //            b.WasDisabled = true;
            //            //b.disabled = true;
            //        }
            //        else if (b.WasDisabled) {
            //            b.WasDisabled = false;
            //            //b.disabled = false;
            //        }
            //}
            //buttons = null;
            if (_touch) return;
            var $buttons = $('button', this._element)
                .each(function (index) {
                    $(this).toggleClass('disabled', !enable);
                });
        },
        _bodyKeydown: function (e) {
            var preventDefault = false;
            if (this._customFilterField) {
                if (e.keyCode == Sys.UI.Key.enter) {
                    preventDefault = true;
                    this.applyCustomFilter();
                }
                else if (e.keyCode == Sys.UI.Key.esc) {
                    preventDefault = true;
                    this.closeCustomFilter();
                }
            }
            else if (this.get_lookupField())
                if (e.keyCode == Sys.UI.Key.esc) {
                    preventDefault = true;
                    this.hideLookup();
                }
            if (preventDefault) {
                e.preventDefault();
                e.stopPropagation();
            }
        },
        _prepareJavaScriptExpressionEx: function (script) {
            return script.replace(/\[((\w+)\.)?(\w+)\]/g, 'this.fieldValue("$3","$2")')
                .replace(/\$row\.(\w+)/gm, 'this.fieldValue("$1")')
                .replace(/\$master\.(\w+)/gm, 'this.fieldValue("$1","master")');
        },
        _evaluateVisibility: function () {
            var script = this.get_visibleWhen();
            if (isNullOrEmpty(script))
                return true;
            script = this._prepareJavaScriptExpressionEx(script);
            return eval(script) != false;
        },
        _filterSourceSelected: function (sender, args, keepContext) {
            this._hidden = !this._evaluateVisibility();
            var vitals = this.readContext('vitals');
            if (vitals) {
                var i = 0;
                while (i < vitals.Filter.length) {
                    var filterExpression = vitals.Filter[i];
                    var filterFieldName = filterExpression.substring(0, filterExpression.indexOf(':'));
                    var isKey = false;
                    if (this._keyFields)
                        for (var j = 0; j < this._keyFields.length; j++)
                            if (this._keyFields[j].Name == filterFieldName) {
                                isKey = true;
                                break;
                            }
                    if (isKey)
                        Array.removeAt(vitals.Filter, i);
                    else
                        i++;
                }
                this.writeContext('vitals', vitals);
            }
            var oldValues = [];
            for (i = 0; i < this._externalFilter.length; i++) {
                Array.add(oldValues, this._externalFilter[i].Value);
                this._externalFilter[i].Value = null;
            }
            var forceHide = false,
                forceChanged = false;
            if (_app.isInstanceOfType(sender)) {
                //if (!isNullOrEmpty(this._transaction))
                //    this.set_transaction(sender.get_transaction());
                this._populateExternalViewFilter(sender);
                forceHide = !sender.get_showDetailsInListMode() && sender.get_isGrid()/*sender.get_viewType() == 'Grid'*/;
                if (sender._forceChanged)
                    forceChanged = true;
            }
            else if (this._externalFilter.length > 0)
                this._externalFilter[0].Value = sender.target.value;
            this.applyExternalFilter(_touch);
            var emptySourceFilter = true,
                sourceFilterChanged = false,
                v, p;
            for (i = 0; i < this._externalFilter.length; i++) {
                v = this._externalFilter[i].Value;
                if (v != null) emptySourceFilter = false;
                if (v != oldValues[i]) sourceFilterChanged = true;
            }
            if (this.get_autoHide() != Web.AutoHideMode.Nothing)
                this._updateLayoutContainerVisibility(!emptySourceFilter && !forceHide && !this._hidden);
            if (sourceFilterChanged || forceChanged) {
                if (!keepContext) {
                    var extension = this.extension();
                    this.set_pageIndex(-1);
                    if (extension && extension.currentPageIndex)
                        extension.currentPageIndex(null);
                }
                p = this._position;
                if (p) {
                    p.index = 0;
                    p.count = -1;
                    p.key = [];
                    p.keyFilter = [];
                    p.filter = this.get_filter();
                }
                if (!keepContext) {
                    this._selectedKey = [];
                    this._selectedKeyFilter = [];
                    this._selectedKeyList = [];
                }
                this._clearCache();
                this.loadPage();
            }
            this.raiseSelected();
            this.updateSummary();
        },
        _createExternalFilter: function () {
            this._externalFilter = [];
            var iterator = /(\w+)(,|$)/g;
            if (this.get_filterFields()) {
                var fieldNames = this.get_filterFields().split(_app._simpleListRegex);
                for (var i = 0; i < fieldNames.length; i++)
                    Array.add(this._externalFilter, { Name: fieldNames[i], Value: null });
            }
        },
        _populateExternalViewFilter: function (view) {
            if (!(view._selectedKey && view._keyFields && view._selectedKey.length == view._keyFields.length)) return;
            for (var i = 0; i < this._externalFilter.length; i++) {
                var filterItem = this._externalFilter[i];
                var found = false;
                for (var j = 0; j < view._keyFields.length; j++) {
                    var field = view._keyFields[j];
                    if (filterItem.Name == field.Name) {
                        filterItem.Value = view.convertFieldValueToString(field, view._selectedKey[j]);
                        found = true;
                        break;
                    }
                }
                if (!found && this.get_controller() != view.get_controller()) {
                    var row = view.get_selectedRow();
                    if (row && row.length)
                        for (j = 0; j < view._allFields.length; j++) {
                            field = view._allFields[j];
                            if (filterItem.Name == field.Name) {
                                filterItem.Value = view.convertFieldValueToString(field, row[view._allFields[j].Index]);
                                found = true;
                                break;
                            }
                        }
                }
                if (!found && view._selectedKey.length >= i)
                    filterItem.Value = view._selectedKey[i];
            }
        },
        _cloneChangedRow: function () {
            if (this.editing()) {
                var values = this._collectFieldValues();
                var selectedRow = this.get_currentRow();
                var row = selectedRow ? Array.clone(selectedRow) : null;
                var designer = __designer();
                for (var i = 0; i < values.length; i++) {
                    var v = values[i];
                    var f = this.findField(v.Name);
                    if (f/* && !f.ReadOnly*/)
                        row[f.Index] = designer ? (v.Modified ? v.NewValue : v.OldValue) : v.NewValue;
                }
                return row;
            }
            else
                return this.get_selectedRow();
        },
        _updateVisibility: function (row) {
            if (!this._expressions) return;
            this._readOnlyChanged = false;
            var isForm = this.get_isForm();
            var expressions = [];
            if (!row)
                row = this._cloneChangedRow();
            if (!row) return;
            var changed = false;
            var checkTabs = false;
            for (var i = 0; i < this._expressions.length; i++) {
                var exp = expressions[0] = this._expressions[i];
                if (exp.Type == Web.DynamicExpressionType.ClientScript)
                    if (exp.Scope == Web.DynamicExpressionScope.DataFieldVisibility) {
                        var f = this.findField(exp.Target);
                        if (f) {
                            var elem = this._get(isForm ? '_ItemContainer' : 'Item', f.Index);
                            if (elem) {
                                var result = this._evaluateJavaScriptExpressions(expressions, row, false);
                                if (isForm) {
                                    var isVisible = Sys.UI.DomElement.getVisible(elem);
                                    if (Sys.UI.DomElement.containsCssClass(elem.parentNode, 'FieldPlaceholder')) elem = elem.parentNode;
                                    else if (Sys.UI.DomElement.containsCssClass(elem.parentNode.parentNode, 'FieldWrapper')) elem = elem.parentNode.parentNode.parentNode.parentNode;
                                    Sys.UI.DomElement.setVisible(elem, result == true);
                                    if (isVisible != result) changed = true;
                                }
                            }
                        }
                    }
                    else if (exp.Scope == Web.DynamicExpressionScope.CategoryVisibility) {
                        var c = this.findCategory(exp.Target);
                        if (c) {
                            elem = this._get('_Category', c.Index);
                            if (elem) {
                                c.IsVisible = this._evaluateJavaScriptExpressions(expressions, row, false);
                                result = c.IsVisible && (isNullOrEmpty(c.Tab) || c.Tab == this._get_selectedTab());
                                isVisible = Sys.UI.DomElement.getVisible(elem);
                                Sys.UI.DomElement.setVisible(elem, result == true);

                                if (isVisible != result) changed = true;
                                var catDescriptionCell = this._get('$CategoryDescription$', c.Index);
                                if (catDescriptionCell) {
                                    var descriptionText = this._processTemplatedText(row, c.Description);
                                    catDescriptionCell.innerHTML = descriptionText;
                                    catDescriptionCell.style.display = isNullOrEmpty(descriptionText) ? 'none' : 'block';
                                }
                                if (!isNullOrEmpty(c.Tab))
                                    checkTabs = true;
                            }
                        }
                    }
                    else if (exp.Scope == Web.DynamicExpressionScope.ReadOnly) {
                        f = this.findField(exp.Target);
                        if (f) {
                            if (f.OriginalTextMode == null)
                                f.OriginalTextMode = f.TextMode;
                            var isReadOnly = f.isReadOnly();
                            result = this._evaluateJavaScriptExpressions(expressions, row, false);
                            f.TextMode = result == true ? 4 : f.OriginalTextMode;
                            if (result != isReadOnly) {
                                changed = true;
                                this._readOnlyChanged = true;
                            }
                        }
                    }
            }
            if (checkTabs)
                for (i = 0; i < this._tabs.length; i++) {
                    var tab = this._tabs[i];
                    var dynamicVisibility = false;
                    var tabIsVisible = false;
                    for (var j = 0; j < this._categories.length; j++) {
                        c = this._categories[j];
                        if (c.Tab == tab && c.IsVisible != null) {
                            dynamicVisibility = true;
                            tabIsVisible = c.IsVisible;
                            if (tabIsVisible)
                                break;
                        }
                    }
                    if (dynamicVisibility)
                        Sys.UI.DomElement.setVisible(this._get('_Tab', i), tabIsVisible);
                }
            if (this._dynamicActionButtons) {
                this._clonedRow = row;
                var topActionButtons = this._get('$ActionButtons$', 'Top');
                if (topActionButtons) {
                    var sb = new Sys.StringBuilder();
                    this._internalRenderActionButtons(sb, 'Top', this._lastActionButtonsScope);
                    this._newTopActionButtons = sb.toString();
                    sb.clear();
                }
                var bottomActionButtons = this._get('$ActionButtons$', 'Bottom');
                if (bottomActionButtons) {
                    sb = new Sys.StringBuilder();
                    this._internalRenderActionButtons(sb, 'Bottom', this._lastActionButtonsScope);
                    this._newBottomActionButtons = sb.toString();
                    sb.clear();
                }
                var self = this;
                setTimeout(function () {
                    self._refreshActionButtons();
                }, 500);
                this._clonedRow = null;
            }
            this._updateStatusBar();
            if (changed) {
                this._adjustModalHeight();
                if (this._modalPopup)
                    this._modalPopup.show();
                _body_performResize();
            }
            if (this.editing() && !this._lookupIsActive) {
                var cell = this._get_focusedCell();
                for (i = 0; i < this._fields.length; i++) {
                    f = this._fields[i];
                    if (!f.ReadOnly && !f.Hidden && f.AutoSelect && f.ItemsStyle == 'Lookup' && (cell == null || cell.colIndex == f.ColIndex)) {
                        f.AutoSelect = false;
                        v = row[f.Index];
                        if (v == null) {
                            this._lookupIsActive = true;
                            self = this;
                            setTimeout(function () {
                                self.showLookup(f.Index);
                            }, 100);
                            break;
                        }
                    }
                }
            }
        },
        _refreshActionButtons: function () {
            var topActionButtons = this._get('$ActionButtons$', 'Top');
            if (topActionButtons && this._newTopActionButtons) {
                topActionButtons.innerHTML = this._newTopActionButtons;
                this._newTopActionButtons = null;
            }
            var bottomActionButtons = this._get('$ActionButtons$', 'Bottom');
            if (bottomActionButtons && this._newBottomActionButtons) {
                bottomActionButtons.innerHTML = this._newBottomActionButtons;
                this._newBottomActionButtons = null;
            }
        },
        _updateDynamicValues: function (field) {
            var that = this,
                done = false,
                allFields = that._allFields,
                fieldName = field && field.Name ? field.Name : field;
            if (field && field.CausesCalculate) {
                if (!that.editing()) {
                    that.refresh();
                    return true;
                }
                that._raiseCalculate(field, field);
                done = true;
            }
            else
                for (var i = 0; i < allFields.length; i++) {
                    var f = allFields[i];
                    var hasContextFields = !isNullOrEmpty(f.ContextFields);
                    if (hasContextFields) {
                        var iterator = /\s*([\w\:]+)\s*(=\s*(\w+)\s*)?(,|$)/g;
                        var m = iterator.exec(f.ContextFields);
                        while (m) {
                            if (f.ItemsAreDynamic && (field == null || m[3] == /*field.Name*/fieldName)) {
                                if (!that.editing()) {
                                    that.refresh();
                                    return true;
                                }
                                that._raisePopulateDynamicLookups();
                                done = true;
                            }
                            //else if (f.Calculated && m[1] == /*field.Name*/fieldName) {
                            //    if (!that.editing()) {
                            //        that.refresh();
                            //        return true;
                            //    }
                            //    that._raiseCalculate(f, field);
                            //    done = true;
                            //}
                            else if ((f.Calculated || f.CausesCalculate) && m[1] == /*field.Name*/fieldName || m[1] == field || 'controller:' + m[1] == field) {
                                if (_touch) {
                                    that.extension().notify(field);
                                    return;
                                }
                                else {
                                    if (!that.editing()) {
                                        that.refresh();
                                        return true;
                                    }
                                    that._raiseCalculate(f, field);
                                    done = true;
                                }
                            }
                            if (done) break;
                            m = iterator.exec(f.ContextFields);
                        }
                        if (done) break;
                    }
                }
            return done;
        },
        _valueFocused: function (index) {
            var field = this._allFields[index];
            this._focusedFieldName = field.Name;
            _app._focusedDataViewId = this._id;
            _app._focusedItemIndex = index;
        },
        _resetContextLookupValues: function (field) {
            var values = [],
                map = {};
            this._enumerateContextFieldValues(field, values, map);
            var result = values.length > 0;
            if (result)
                this.refresh(true, values);
            return result;
        },
        _enumerateContextFieldValues: function (field, values, map, row) {
            var fieldName = field.Name, m, listedFields = {},
                itemsDataController = field.ItemsDataController;
            if (itemsDataController) {
                $(values).each(function () {
                    listedFields[this.name || this.Name] = true;
                });
                var allFields = this._allFields;
                for (var i = 0; i < allFields.length; i++) {
                    var f = allFields[i];
                    var contextFields = f.ContextFields;
                    if (contextFields && !map[i] && f.ItemsStyle) {
                        var iterator = /\s*(\w+)\s*(=\s*(\w+)\s*)?(,|$)/g;
                        //var m = iterator.exec(contextFields);
                        while (m = iterator.exec(contextFields)) {
                            if (m[3] == fieldName) {
                                var dependentItemsDataController = f.ItemsDataController,
                                    requiresReset = !(itemsDataController && dependentItemsDataController) || itemsDataController != dependentItemsDataController;
                                map[i] = true;
                                if (!listedFields[f.Name])
                                    values.push({ 'name': f.Name, 'value': !requiresReset && row ? row[f.Index] : null });
                                var aliasField = allFields[f.AliasIndex];
                                if (aliasField != f && !listedFields[aliasField.Name])
                                    values.push({ 'name': aliasField.Name, 'value': !requiresReset && row ? row[aliasField.Index] : null });
                                var copy = f.Copy,
                                    m2;
                                if (copy && requiresReset)
                                    while (m2 = _app._fieldMapRegex.exec(copy))
                                        if (!listedFields[m2[1]])
                                            values.push({ 'name': m2[1], 'value': null });
                                if (!f.skipPopulate) {
                                    f.DynamicItems = null;
                                    f.ItemCache = null;
                                }
                                this._enumerateContextFieldValues(f, values, map, row);
                            }
                            //m = iterator.exec(contextFields);
                        }
                    }
                }
            }
        },
        _copyStaticLookupValues: function (field) {
            if (!isNullOrEmpty(field.Copy) && (field.ItemsStyle == 'RadioButtonList' || field.ItemsStyle == 'ListBox' || field.ItemsStyle == 'DropDownList')) {
                var currentRow = this._collectFieldValues(),
                    selectedValue = currentRow[field.Index].NewValue,
                    selectedItem = null,
                    items = field.DynamicItems || field.Items;
                if (selectedValue != null && typeof (selectedValue) != 'string')
                    selectedValue = selectedValue.toString();
                for (var i = 0; i < items.length; i++) {
                    var item = items[i],
                        itemValue = item[0];
                    if (itemValue != null && typeof (itemValue) != 'string') itemValue = itemValue.toString();
                    if (itemValue == selectedValue) {
                        selectedItem = item;
                        break;
                    }
                }
                if (selectedItem) {
                    var values = [],
                        index = 2, m;
                    while (m = _app._fieldMapRegex.exec(field.Copy))
                        Array.add(values, { 'name': m[1], 'value': selectedItem[index++] });
                    if (_touch)
                        //this.extension().afterCalculate(values);
                        _app.input.execute({ dataView: this, values: values });
                    else
                        this.refresh(true, values);
                    return true;
                }
            }
            return false;
        },
        _replaceFieldValue: function (field) {
            var expressions = this._enumerateExpressions(Web.DynamicExpressionType.RegularExpression, Web.DynamicExpressionScope.Field, field.Name);
            for (var j = 0; j < expressions.length; j++) {
                var exp = expressions[j];
                try {
                    if (exp.Result.match(/\$(\d|\'\`)/)) {
                        var currentRow = this._collectFieldValues();
                        var v = currentRow[field.Index];
                        var s = v.NewValue ? v.NewValue.toString() : null;
                        var re = new RegExp(exp.Test);
                        var s2 = s.replace(re, exp.Result);
                        if (s2 != s) {
                            var values = [{ 'name': field.Name, 'value': s2 }];
                            this.refresh(true, values);
                        }
                    }
                }
                catch (ex) {
                }
            }
        },
        _valueChanged: function (index) {
            var field = this._allFields[index];
            this._showFieldError(field, null);
            this._skipErrorReset = false;
            var self = this;
            self._valueChangedTimeout = setTimeout(function () {
                if (!_app._navigated)
                    self._performValueChanged(index, true);
            }, 200);
        },
        _performValueChanged: function (index, processReadOnly) {
            var field = this._allFields[index];
            if (!field) return;
            if (this._skipErrorReset) {
                this._skipErrorReset = false;
                return;
            }
            else
                this._showFieldError(field, null);
            this._replaceFieldValue(field);
            var r3 = this._resetContextLookupValues(field);
            var r1 = this._copyStaticLookupValues(field);
            this._updateVisibility();
            var r2 = this._updateDynamicValues(field);
            if (processReadOnly && this._readOnlyChanged && this.editing()) {
                this.refresh(true);
                this._focus();
            }
            return r1 || r2 || r3 || this._readOnlyChanged;
        },
        _quickFind_focus: function (e) {
            var qf = this.get_quickFindElement();
            if (qf.value == resources.Grid.QuickFindText)
                qf.value = '';
            Sys.UI.DomElement.removeCssClass(qf, 'Empty');
            Sys.UI.DomElement.removeCssClass(qf, 'NonEmpty');
            qf.select();
            this._lostFocus = true;
        },
        _quickFind_blur: function (e) {
            var qf = this.get_quickFindElement();
            if (isBlank(qf.value)) {
                qf.value = resources.Grid.QuickFindText;
                Sys.UI.DomElement.addCssClass(qf, 'Empty');
            }
            else
                Sys.UI.DomElement.addCssClass(qf, 'NonEmpty');
            this._lostFocus = false;
        },
        _executeQuickFind: function (qry) {
            this._forceSync();
            for (var i = 0; i < this._allFields.length; i++)
                this._allFields[i]._listOfValues = null;
            this.removeQuickFind();
            for (i = 0; i < this._allFields.length; i++) {
                var f = this._allFields[i];
                if (!f.Hidden) {
                    f = this._allFields[f.AliasIndex];
                    if (isNullOrEmpty(qry)) {
                        //this.removeFromFilter(f);
                        this.refreshData();
                        //                    this.set_pageIndex(-2);
                        //                    this._loadPage();
                    }
                    else
                        this.applyFilter(f, '~', qry);
                    break;
                }
            }
            //this._forgetSelectedRow(true);
        },
        quickFind: function (sample) {
            var q = (isNullOrEmpty(sample) ? this.get_quickFindElement().value : sample).match(/^\s*(.*?)\s*$/);
            var qry = q[1] == resources.Grid.QuickFindText ? '' : q[1];
            this.set_quickFindText(qry);
            this._executeQuickFind(qry);
            this._lostFocus = false;
        },
        _quickFind_keydown: function (e) {
            if (e.keyCode == Sys.UI.Key.enter) {
                e.preventDefault();
                e.stopPropagation();
                this.quickFind();
            }
            else if (e.keyCode == Sys.UI.Key.down) {
                return;
                //if (this.get_isDataSheet() && this._totalRowCount > 0) {
                //    e.preventDefault();
                //    e.stopPropagation();
                //    if (!this._get_focusedCell())
                //        this._startInputListenerOnCell(0, 0);
                //    else
                //        this._lostFocus = false;
                //    var elem = this._focusCell(-1, -1, true);
                //    elem.focus();
                //}
            }
        },
        encodePermalink: function (link, target, features) {
            Sys.Net.WebServiceProxy.invoke(this.get_servicePath(), 'EncodePermalink', false, { 'link': link, 'rooted': false }, Function.createDelegate(this, this._encodePermalink_Success), Function.createDelegate(this, this._onMethodFailed), { 'target': target, 'features': features });
        },
        _encodePermalink_Success: function (result, context) {
            if (context.target || context.features)
                open(result, context.target, context.features)
            else
                location.href = result;
        },
        showViewMessage: function (message) {
            if (_touch)
                _touch.notify({ text: message, force: true });
            else {
                var elem = this._get('$HeaderText');
                if (elem != null && message) {
                    var headerText = String.format('<div class="MsgBox">{1} <a href="javascript:" onclick="$find(\'{0}\').hideViewMessage();return false;" class="Close" title="{2}">&nbsp;</a></div>', this.get_id(), message, resourcesModalPopup.Close);
                    var view = this.get_view();
                    if (!view.OriginalHeaderText)
                        view.OriginalHeaderText = view.HeaderText;
                    view.HeaderText = headerText;
                    elem.innerHTML = headerText;
                    if (!this._viewMessages)
                        this._viewMessages = {};
                    this._viewMessages[view.Id] = message;

                }
            }
        },
        hideViewMessage: function () {
            var elem = this._get('$HeaderText');
            if (elem != null) {
                var view = this.get_view();
                var text = view.OriginalHeaderText;
                if (!text) return;
                view.HeaderText = text;
                elem.innerHTML = this._formatViewText(resources.Views.DefaultDescriptions[text], true, text);
                this._viewMessages[view.Id] = null;
            }
        }
    }

    _app.loadDesigner = function () {
        if (!_app.Designer) {
            // load css
            var cssLink = 'http://localhost:' + __designerPort + '/css/codeontime.designer.css';
            if (!$('link[href="' + cssLink + '"]').length)
                $('head').append('<link id="designer-css" rel="stylesheet" type="text/css" href="' + cssLink + '">');

            // load script
            if (typeof (__cotdesigner) != 'undefined')
                _app.Designer = new __cotdesigner;
            else {
                $.ajax('http://localhost:' + __designerPort + '/css/codeontime.designer.css');
                $.getScript('http://localhost:' + __designerPort + '/js/codeontime.designer.js', function () {
                    if (typeof (__cotdesigner) != 'undefined') _app.Designer = new __cotdesigner;
                });
            }
        }
        else
            _app.Designer.open();
    }

    _app.registerClass('Web.DataView', Sys.UI.Behavior);

    _web.AutoComplete = function (element) {
        Web.AutoComplete.initializeBase(this, [element]);
    }

    _web.AutoComplete.prototype = {
        initialize: function () {
            Web.AutoComplete.callBaseMethod(this, 'initialize');
            this._textBoxMouseOverHandler = Function.createDelegate(this, this._onTextBoxMouseOver);
            this._textBoxMouseOutHandler = Function.createDelegate(this, this._onTextBoxMouseOut);
            this._completionListItemCssClass = 'Item';
            this._highlightedItemCssClass = 'HighlightedItem'
        },
        dispose: function () {
            this._viewPage = null;
            if (this._element) {
                $removeHandler(this._element, 'mouseover', this._textBoxMouseOverHandler);
                $removeHandler(this._element, 'mouseout', this._textBoxMouseOutHandler);
            }
            Web.AutoComplete.callBaseMethod(this, 'dispose');
        },
        get_fieldName: function () {
            return this._fieldName;
        },
        set_fieldName: function (value) {
            this._fieldName = value;
        },
        updated: function () {
            var f = document.createElement('div');
            f.className = 'AutoCompleteFrame ' + this.get_typeCssClass();
            f.innerHTML = String.format('<table><tr><td class="Input"></td><td class="Clear" style="{2}"><span class="Clear" onclick="var e=this.parentNode.parentNode.getElementsByTagName(\'input\')[0];e.value=\'\';e.focus()" title="{1}">&nbsp;</span></td><td class="Button" onmouseover="if(!Sys.UI.DomElement.containsCssClass(this.parentNode, \'Active\'))$find(\'{0}\')._showDropDown(true)" onmouseout="$find(\'{0}\')._showDropDown(false)"><span class="Button" onclick="$find(\'{0}\')._showFullList();">&nbsp;</span></td></tr></table>', this.get_id(), resourcesDataFiltersLabels.Clear, this.get_contextKey().match(/^(SearchBar|Filter)\:/) != null ? '' : 'display:none');
            this._element.setAttribute('autocomplete', 'off');
            this._element.parentNode.insertBefore(f, this._element);
            f.getElementsByTagName('td')[0].appendChild(this._element);
            if (Sys.Browser.agent == Sys.Browser.WebKit)
                f.style.marginLeft = '2px';
            if (this._completionSetCount == 10)
                this._completionSetCount = 100;
            $addHandler(this._element, 'mouseover', this._textBoxMouseOverHandler);
            $addHandler(this._element, 'mouseout', this._textBoxMouseOutHandler);
            Web.AutoComplete.callBaseMethod(this, 'updated');
            document.body.appendChild(this._completionListElement);
            this._completionListElement.className = 'CompletionList AutoComplete';
            this._completionListElement.style.display = 'none';
        },
        _showCachedFullList: function () {
            this._update('%', this._cache['%'], false);
        },
        _showFullList: function () {
            if (this._webRequest) return;
            var visible = this._popupBehavior.get_visible();
            this._hideCompletionList();
            this._element.focus();
            if (visible) return;
            this._completionWord = '%';
            if (this._cache && this._cache['%']) {
                var self = this;
                setTimeout(function () {
                    self._showCachedFullList();
                }, 50);
                return;
            }
            var params = { prefixText: this._completionWord, count: this._completionSetCount };
            if (this._useContextKey) params.contextKey = this._contextKey;
            this._ignoreCompletionWord = true;
            this._invoke(params, this._completionWord);
        },
        _showDropDown: function (show) {
            var e = this._element;
            while (e.tagName != 'TR')
                e = e.parentNode;
            if (show)
                Sys.UI.DomElement.addCssClass(e, 'Active');
            else if (this._textBoxHasFocus)
                Sys.UI.DomElement.addCssClass(e, 'Active');
            else
                Sys.UI.DomElement.removeCssClass(e, 'Active');
        },
        _onKeyDown: function (ev) {
            if (ev.keyCode == Sys.UI.Key.down && ev.altKey) {
                ev.preventDefault();
                ev.stopPropagation();
                this._showFullList();
            }
            var popupVisible = this._popupBehavior._visible;
            Web.AutoComplete.callBaseMethod(this, '_onKeyDown', [ev]);
            if (ev.keyCode == Sys.UI.Key.enter || ev.keyCode == Sys.UI.Key.esc) {
                var dataView = this._get_fieldDataView(true);
                if (dataView)
                    if (dataView.get_isDataSheet() && popupVisible) {
                        ev.preventDefault();
                        ev.stopPropagation();
                    }
                    else if (dataView.get_searchBarIsVisible()) {
                        ev.preventDefault();
                        ev.stopPropagation();
                        if (!popupVisible)
                            setTimeout(function () {
                                dataView._performSearch();
                            }, 100);
                    }
            }
        },
        _update: function (prefixText, completionItems, cacheResults) {
            Web.AutoComplete.callBaseMethod(this, '_update', [prefixText, completionItems, cacheResults]);
            if (completionItems) {
                var index = -1;
                var w = this._currentCompletionWord().toLowerCase();
                for (var i = 0; i < completionItems.length; i++) {
                    var s = completionItems[i];
                    if (s != null) {
                        s = s.toLowerCase();
                        if (index == -1 && s.startsWith(w))
                            index = i;
                        if (s == w) {
                            index = i;
                            break;
                        }
                    }
                }
                //var index = Array.indexOf(completionItems, this._currentCompletionWord());
                if (index >= 0 && index < this._completionListElement.childNodes.length) {
                    this._selectIndex = index;
                    w = this._completionListElement.childNodes[index];
                    this._highlightItem(w);
                    this._handleScroll(w, index + 5);
                }
            }
        },
        _get_fieldDataView: function (allTypes) {
            var dataView = null;
            var info = this._get_contextInfo();
            if (info && (info.type == 'Field' || allTypes && info.type == 'SearchBar'))
                dataView = _app.find(info.controller);
            return dataView;
        },
        _setText: function (item) {
            Web.AutoComplete.callBaseMethod(this, '_setText', [item]);
            this._updateClearButton();
            //var info = this._get_contextInfo();
            var dataView = this._get_fieldDataView();
            if (dataView/*info && info.type == 'Field'*/) {
                //var dataView = _app.find(info.controller);
                var field = dataView.findField(this.get_fieldName());
                var w = this.get_element().value;
                var values = [];
                if (w != resourcesData.NullValueInForms) {
                    var index = this._enumerateViewPageItems(w);
                    if (index != -1) {
                        var page = this._viewPage;
                        var r = page.Rows[index];
                        var valueFields = [];
                        for (var i = 0; i < page.Fields.length; i++) {
                            var f = page.Fields[i];
                            if (f.Name == field.ItemsDataValueField)
                                Array.add(valueFields, f);
                        }
                        if (valueFields.length == 0)
                            for (i = 0; i < page.Fields.length; i++) {
                                f = page.Fields[i];
                                if (f.IsPrimaryKey) {
                                    Array.add(valueFields, f);
                                    break;
                                }
                            }
                        for (i = 0; i < valueFields.length; i++) {
                            var v = r[valueFields[0].Index]
                            Array.add(values, v);
                        }
                    }
                }
                $get(dataView.get_id() + '_Item' + field.Index).value = values.toString();
                this._originalElementText = this.get_element().value;
                if (this._get_isInLookupMode()) {
                    if (!isNullOrEmpty(field.Copy)) {
                        values = [];
                        var iterator = /(\w+)=(\w+)/g;
                        var m = iterator.exec(field.Copy);
                        while (m) {
                            if (m[2] == 'null')
                                Array.add(values, { 'name': m[1], 'value': null });
                            else
                                for (i = 0; i < page.Fields.length; i++) {
                                    if (page.Fields[i].Name == m[2])
                                        Array.add(values, { 'name': m[1], 'value': r[i] });
                                }
                            m = iterator.exec(field.Copy);
                        }
                        dataView.refresh(true, values);
                    }
                    dataView._valueChanged(field.Index);
                }
            }
        },
        _get_isInLookupMode: function () {
            var info = this._get_contextInfo();
            return info != null && info.type == 'Field';
        },
        _updateClearButton: function () {
            var tr = this._element.parentNode.parentNode;
            if (!isBlank(this._element.value))
                Sys.UI.DomElement.addCssClass(tr, 'Filtered');
            else
                Sys.UI.DomElement.removeCssClass(tr, 'Filtered');
        },
        _onGotFocus: function (ev) {
            Web.AutoComplete.callBaseMethod(this, '_onGotFocus', [ev]);
            this._showDropDown(true);
            this._updateClearButton();
            if (this._get_isInLookupMode()) {
                var elem = this.get_element();
                this._originalElementText = elem.value;
                if (this._originalElementText == resourcesData.NullValueInForms) {
                    elem.value = '';
                    elem.select();
                }
            }
        },
        _onLostFocus: function (ev) {
            Web.AutoComplete.callBaseMethod(this, '_onLostFocus', [ev]);
            this._showDropDown(false);
            this._updateClearButton();
            if (this._get_isInLookupMode() && this._originalElementText != null)
                this.get_element().value = this._originalElementText;
        },
        _onTextBoxMouseOver: function (ev) {
            this._showDropDown(true);
        },
        _onTextBoxMouseOut: function (ev) {
            this._showDropDown(false);
        },
        _currentCompletionWord: function () {
            if (this._completionWord) {
                var w = this._completionWord;
                this._completionWord = null;
                return w;
            }
            return Web.AutoComplete.callBaseMethod(this, '_currentCompletionWord');
        },
        _onTimerTick: function (sender, eventArgs) {
            // turn off the timer until another key is pressed.
            this._timer.set_enabled(false);
            if (this._servicePath && this._serviceMethod) {
                var text = this._currentCompletionWord();

                if (text.trim().length < this._minimumPrefixLength) {
                    this._currentPrefix = null;
                    this._update('', null, /* cacheResults */false);
                    return;
                }
                // there is new content in the textbox or the textbox is empty but the min prefix length is 0
                if ((this._currentPrefix !== text) || ((text == "") && (this._minimumPrefixLength == 0))) {
                    this._currentPrefix = text;
                    if ((text != "") && this._cache && this._cache[text]) {
                        this._update(text, this._cache[text], /* cacheResults */false);
                        return;
                    }
                    // Raise the populating event and optionally cancel the web service invocation
                    eventArgs = new Sys.CancelEventArgs();
                    this.raisePopulating(eventArgs);
                    if (eventArgs.get_cancel()) {
                        return;
                    }

                    // Create the service parameters and optionally add the context parameter
                    // (thereby determining which method signature we're expecting...)
                    var params = { prefixText: this._currentPrefix, count: this._completionSetCount };
                    if (this._useContextKey) {
                        params.contextKey = this._contextKey;
                    }

                    if (this._webRequest) {
                        // abort the previous web service call if we 
                        // are issuing a new one and the previous one is 
                        // active.
                        this._webRequest.get_executor().abort();
                        this._webRequest = null;
                    }
                    // Invoke the web service
                    this._invoke(params, text);
                    $common.updateFormToRefreshATDeviceBuffer();
                }
            }
        },
        _get_contextInfo: function () {
            var m = this.get_contextKey().match(/^(\w+)\:(\w+),(\w+)$/);
            return m ? { 'type': m[1], 'controller': m[2], 'fieldName': m[3] } : null;
        },
        _invoke: function (params, text) {
            var that = this,
                info = that._get_contextInfo();
            if (info) {
                var dataView = _app.find(info.controller);
                var filter = [];
                var searchFieldName = info.fieldName;
                var operation = 'beginswith';
                if (info.type == 'SearchBar')
                    filter = dataView._createSearchBarFilter(true);
                else if (info.type == 'Filter')
                    filter = dataView.get_filter();
                else {
                    var field = dataView.findField(this.get_fieldName());
                    searchFieldName = !isNullOrEmpty(field.ItemsDataTextField) ? field.ItemsDataTextField : field.Name;
                }
                if (!this._ignoreCompletionWord) {
                    for (var i = 0; i < filter.length; i++) {
                        var fm = filter[i].match(/^(\w+):/);
                        if (fm[1] == info.fieldName) {
                            Array.removeAt(filter, i);
                            break;
                        }
                    }
                    if (!field)
                        field = dataView.findField(searchFieldName);
                    if (field && field.AutoCompleteAnywhere)
                        operation = 'contains';
                    Array.add(filter, String.format('{0}:${1}${2}\0', searchFieldName, operation, this._currentCompletionWord()));
                }
                var r = null,
                    sourceController = null,
                    sourceView = null,
                    fieldFilter, copyInfo, copyIterator;
                if (info.type == 'Field') {
                    sourceController = field.ItemsDataController;
                    sourceView = field.ItemsDataView || 'grid1';
                    fieldFilter = [field.ItemsDataTextField];
                    if (field.Copy)
                        while (copyInfo = _app._fieldMapRegex.exec(field.Copy))
                            fieldFilter.push(copyInfo[2]);
                    var lc = { 'FieldName': field.Name, 'Controller': dataView.get_controller(), 'View': dataView.get_viewId() };
                    var contextFilter = dataView.get_contextFilter(field);
                    for (i = 0; i < contextFilter.length; i++) {
                        var cfv = contextFilter[i];
                        Array.add(filter, String.format('{0}:={1}\0', cfv.Name, cfv.Value));
                    }
                    r = {
                        PageIndex: 0,
                        RequiresMetaData: true,
                        RequiresRowCount: false,
                        PageSize: 300,
                        FieldFilter: fieldFilter,
                        SortExpression: field.ItemsDataTextField,
                        Filter: filter,
                        ContextKey: dataView.get_id(),
                        //Cookie: dataView.get_cookie(),
                        FilterIsExternal: contextFilter.length > 0,
                        //Transaction: dataView.get_transaction(),
                        LookupContextFieldName: lc ? lc.FieldName : null,
                        LookupContextController: lc ? lc.Controller : null,
                        LookupContextView: lc ? lc.View : null,
                        LookupContext: lc,
                        View: dataView.get_viewId(),
                        Tag: dataView.get_tag(),
                        MetadataFilter: ['fields'],
                        ExternalFilter: contextFilter
                    };
                }
                else {
                    sourceController = dataView.get_controller();
                    sourceView = dataView.get_viewId();
                    r = {
                        FieldName: info.fieldName,
                        Filter: /*filter.length == 1 && filter[0].match(/(\w+):/)[1] == m[3] ? null : */filter,
                        LookupContextFieldName: lc ? lc.FieldName : null,
                        LookupContextController: lc ? lc.Controller : null,
                        LookupContextView: lc ? lc.View : null,
                        AllowFieldInFilter: this._ignoreCompletionWord != true,
                        Controller: sourceController,
                        View: sourceView,
                        Tag: dataView.get_tag(),
                        ExternalFilter: dataView.get_externalFilter()
                    };
                }
                dataView._busy(true);
                that._webRequest = dataView._invoke(that.get_serviceMethod(), { controller: sourceController, view: sourceView, request: r },
                    function (result, context) {
                        if (info.type == 'Field') {
                            dataView._busy(false);
                            that._onGetPageComplete(result, context);
                        }
                        else {
                            dataView._busy(false);
                            that._onGetListOfValuesComplete(result, context);
                        }
                    },
                    text);
                that._ignoreCompletionWord = false;
            }
            else
                that._webRequest = Sys.Net.WebServiceProxy.invoke(that.get_servicePath(), that.get_serviceMethod(), false, params,
                    Function.createDelegate(that, that._onMethodComplete),
                    Function.createDelegate(that, that._onMethodFailed),
                    text);
        },
        _onGetListOfValuesComplete: function (result, context) {
            if (result.length > 0 && result[0] == null)
                result[0] = resourcesHeaderFilter.EmptyValue;
            this._webRequest = null; // clear out our saved WebRequest object    
            this._update(context, result, /* cacheResults */true);
        },
        _onGetPageComplete: function (result, context) {
            if (!this._element) return;
            this._viewPage = result;
            var listOfValues = this._enumerateViewPageItems();
            this._onGetListOfValuesComplete(listOfValues, context);
        },
        _enumerateViewPageItems: function (matchText) {
            var page = this._viewPage;
            //var info = this._get_contextInfo();
            //var dataView = _app.find(info.controller);
            var dataView = this._get_fieldDataView();
            var field = dataView.findField(this.get_fieldName());
            var textFields = [];
            for (var i = 0; i < page.Fields.length; i++) {
                var f = page.Fields[i];
                f.Index = i;
                if (!f.Type)
                    f.Type == 'String';
                if (f.Name == field.ItemsDataTextField)
                    Array.add(textFields, f);
            }
            if (textFields.length == 0)
                for (i = 0; i < page.Fields.length; i++) {
                    f = page.Fields[i];
                    if (!f.Hidden && f.Type == 'String') {
                        Array.add(textFields, f);
                        break;
                    }
                }
            if (textFields.length == 0)
                for (i = 0; i < page.Fields.length; i++) {
                    f = page.Fields[i];
                    if (!f.Hidden) {
                        Array.add(textFields, f);
                        break;
                    }
                }
            var listOfValues = [];
            if (field.AllowNulls)
                Array.add(listOfValues, resourcesData.NullValueInForms);
            for (i = 0; i < page.Rows.length; i++) {
                var v = page.Rows[i][textFields[0].Index];
                if (matchText != null) {
                    if (v == matchText)
                        return i;
                }
                else
                    Array.add(listOfValues, v);
            }
            return matchText != null ? -1 : listOfValues;
        },
        get_typeCssClass: function () {
            return this._typeCssClass;
        },
        set_typeCssClass: function (value) {
            this._typeCssClass = value;
        }
    }

    _app.hideMessage = function () { _app.showMessage() }

    _app.formatMessage = function (type, message) { return _touch && false ? message : String.format('<table cellpadding="0" cellspacing="0" ><tr><td class="{0}" valign="top">&nbsp;</td><td class="Message">{1}</td></tr></table>', type, message) }

    _app.showMessage = function (message) {
        if (_touch)
            _touch.notify({ text: message, force: true, duration: 'medium' });
        else {
            if (isBlank(message)) message = null;
            var bodyTag = document.getElementsByTagName('body')[0];
            var messageIsVisible = false;
            if (!_app.MessageBar) {
                var panel = document.createElement('div');
                panel.id = 'DataView_MessageBar';
                bodyTag.appendChild(panel);
                Sys.UI.DomElement.setVisible(panel, false);
                Sys.UI.DomElement.addCssClass(panel, 'MessageBar');
                _app.MessageBar = $create(AjaxControlToolkit.AlwaysVisibleControlBehavior, { VerticalOffset: AjaxControlToolkit.VerticalSide.Top }, null, null, panel);
                var b = Sys.UI.DomElement.getBounds(bodyTag);
                if (b.y < 0) b.y = 0;
                _app.OriginalBodyTopOffset = b.y;
            }
            panel = $get('DataView_MessageBar');
            var visible = Sys.UI.DomElement.getVisible(panel);
            panel.innerHTML = message ? String.format('<div>{0}</div><div class="Stub"></div>', message) : '';
            Sys.UI.DomElement.setVisible(panel, message != null);
            var bounds = Sys.UI.DomElement.getBounds(panel);
            var bodyTop = message ? _app.OriginalBodyTopOffset + bounds.height : _app.OriginalBodyTopOffset;
            bodyTag.style.paddingTop = bodyTop + 'px';
            var loginDialog = $get("Membership_Login");
            if (loginDialog) loginDialog.style.marginTop = (bodyTop) + 'px';
            if (Sys.UI.DomElement.getVisible(panel) != visible) _body_performResize();
        }
    }

    _app._tagsWithIndexes = new Array('A', 'AREA', 'BUTTON', 'INPUT', 'OBJECT', 'SELECT', 'TEXTAREA', 'IFRAME');
    _app._delayedLoadingViews = [];

    _app._performDelayedLoading = function () {
        var i = 0;
        while (i < _app._delayedLoadingViews.length) {
            var v = _app._delayedLoadingViews[i];
            if (v.get_isDisplayed()) {
                Array.remove(_app._delayedLoadingViews, v);
                if (v._delayedLoading)
                    v._loadPage();
            }
            else i++;
        }
    }

    _app.unanchor = function (href) {
        var m = href.match(/^(.+?)#.*$/);
        return m ? m[1] : href;
    }

    function validateDate(date) {
        return date && date.getTimezoneOffset && !isNaN(+date);
    }

    _app.ensureJSONProperties = ['OldValue', 'NewValue', 'Value'];
    _app.ensureJSONCompatibility = function (values) {
        $(values).each(function () {
            var v = this;
            $(_app.ensureJSONProperties).each(function () {
                var p = this;
                if (v[p]) {
                    if (v[p].getTimezoneOffset)
                        v[p] = _app.stringifyDate(v[p]);
                    //else if (typeof(v[p]) == 'string')
                    //    v[p] = _app.htmlEncode(v[p]);
                }
            });
        });

    }


    _app.stringifyDate = function (d) {
        if (!validateDate(d))
            return d;

        //if (field.Type == "DateTimeOffset") {
        //    var offset = -d.getTimezoneOffset(),
        //        sym = offset >= 0 ? '+' : '-';
        //    return String.format('{0}-{1}-{2}T{3}:{4}:{5}{6}{7}:{8}',
        //        d.getFullYear(), pad(d.getMonth() + 1), pad(d.getDate()), pad(d.getHours()), pad(d.getMinutes()), pad(d.getSeconds()), sym, pad(offset / 60), pad(offset % 60));
        //}

        //else
        //    return String.format('{0}-{1}-{2}T{3}:{4}:{5}',
        //        d.getFullYear(), pad(d.getMonth() + 1), pad(d.getDate()), pad(d.getHours()), pad(d.getMinutes()), pad(d.getSeconds()));
        var convert = dateTimeFormat.Calendar.convert,
            dn = +d;
        if (convert) {
            var nd = convert.toGregorian(d.getFullYear(), d.getMonth(), d.getDate());
            if (nd) {
                nd.setHours(d.getHours(), d.getMinutes(), d.getSeconds(), d.getMilliseconds());
                d = nd;
            }
        }
        return String.format('{0}-{1:d2}-{2:d2}T{3:d2}:{4:d2}:{5:d2}.{6:d3}',
            d.getFullYear(), d.getMonth() + 1, d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds(), d.getMilliseconds());
    }

    _app.isISO8601DateString = function (value) {
        return value && typeof value == 'string' && value.match(/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/);
    }

    _app.csv = {
        delimiter: ',',
        toString: function (ar) {
            ar.forEach(function (v, index) {
                if (v == null)
                    v = '';
                else {
                    v = v.toString();
                    var quotes;
                    if (v.match(/\"/)) {
                        quotes = true
                        v = v.replace(/\"/g, '""');
                    }
                    else if (v.match(/(\r|\n|,)/))
                        quotes = true;
                    if (quotes)
                        v = '"' + v + '"';
                }
                ar[index] = v;

            });
            return ar.join(',');
        },
        toArray: function (s) {
            return _app.csv.toData(s)[0];
        },
        toData: function (s) {
            var delimiter = _app.csv.delimiter,
                data = [[]],
                arrMatches = null,
                d, v,
                pattern = new RegExp(
                    // Delimiters.
                    '(\\' + delimiter + '|\\r?\\n|\\r|^)' +
                    // Quoted fields.
                    '(?:\"([^\"]*(?:\"\"[^\"]*)*)\"|' +
                    // Standard fields.
                    '([^\"\\' + delimiter + '\\r\\n]*))', 'gi');

            // Keep looping over the regular expression matches
            // until we can no longer find a match.
            if (s != null && s.length)
                while (arrMatches = pattern.exec(s)) {
                    // Get the delimiter that was found.
                    d = arrMatches[1];

                    // Check to see if the given delimiter has a length
                    // (is not the start of string) and if it matches
                    // field delimiter. If id does not, then we know
                    // that this delimiter is a row delimiter.
                    if (d.length && d !== delimiter)
                        // Since we have reached a new row of data,
                        // add an empty row to our data array.
                        data.push([]);

                    // Now that we have our delimiter out of the way,
                    // let's check to see which kind of value we
                    // captured (quoted or unquoted).
                    if (arrMatches[2])
                        // We found a quoted value. When we capture
                        // this value, unescape any double quotes.
                        v = arrMatches[2].replace(
                            new RegExp("\"\"", "g"),
                            "\""
                        );
                    else
                        // We found a non-quoted value.
                        v = arrMatches[3];

                    // Now that we have our value string, let's add
                    // it to the data array.
                    data[data.length - 1].push(v);
                }
            return data;
        }
    };

    _app.execute = function (args) {
        if (args.done)
            args.success = args.done;
        if (args.fail)
            args.error = args.fail;
        var placeholder = $('<p>'),
            externalFilter = args.externalFilter,
            dataView = $create(Web.DataView, {
                controller: args.controller, viewId: args.view || 'grid1',
                servicePath: args.url || (typeof __servicePath == 'string' ? __servicePath : ''), baseUrl: (typeof __baseUrl == 'string' ? __baseUrl : ''), useCase: '$app', tags: args.tags,
                externalFilter: externalFilter, filterSource: args.filterSource || (externalFilter && externalFilter.length ? 'External' : null)
            }, null, null, placeholder.get(0)),
            sync = args.sync,
            api = {},
            fieldName,
            requiresNewRow = args.requiresNewRow,
            deferred = $.Deferred();
        if (args.nativeDates == false)
            _app._deserializeDates = false;
        dataView._api = api;
        if (args.odp != false)
            dataView.odp = _app.odp.get();
        if (sync)
            dataView._syncKey = Array.isArray(sync) ? sync : [sync];
        if (args.requiresData == false)
            api.DoesNotRequireData = true;
        if (args.background)
            api.background = true;
        api.requiresAggregates = args.requiresAggregates == true;
        api.metadataFilter = args.metadataFilter || ['fields'];
        if (args.fieldFilter)
            api.fieldFilter = args.fieldFilter;

        function cleanup() {
            dataView.dispose();
            placeholder.remove();
        }

        function done() {
            dataView._busy(false);
        }

        function resolve(r) {
            if (args.nativeDates == false)
                _app._deserializeDates = true;
            if (args.success)
                args.success(r);
            else
                deferred.resolve(r);

        }

        dataView._onMethodFailed = function (error, response, method) {
            done();
            _app._deserializeDates = true;
            if (args.error)
                args.error(error, response);
            else
                deferred.reject(error, response);
            cleanup();
        }

        dataView._onGetPageComplete = function (result) {
            var batchMode = !!args.batch;
            if (!batchMode)
                done();
            var r = { totalRowCount: result.TotalRowCount, pageIndex: result.PageIndex, pageSize: result.PageSize },
                rows = r[result.Controller] = [],
                requiresFormatting = args.format,
                pk = [], fieldMap = {};
            $(result.Fields).each(function (index) {
                var f = this;
                if (f.IsPrimaryKey)
                    pk.push(f);
                configureDefaultProperties(f);
                f.OriginalIndex = f.AliasIndex = f.Index = index;
                fieldMap[f.Name] = f;
            });
            $(result.Fields).each(function (index) {
                var f = this,
                    af;
                if (f.AliasName) {
                    af = fieldMap[f.AliasName];
                    if (af) {
                        f.AliasIndex = af.Index;
                        af.OriginalIndex = f.Index;
                    }
                }
            });
            //if (requiresFormatting)
            $(result.Fields).each(function () {
                var f = this;
                _field_prepareDataFormatString(dataView, f);
                f.format = _field_format;
            });
            r.primaryKey = pk;
            r.fields = result.Fields;
            r.map = fieldMap;
            if (requiresNewRow)
                result.Rows = [result.NewRow];
            $(result.Rows).each(function (index) {
                var dataRow = this,
                    newRow = {};
                rows.push(newRow);
                $(result.Fields).each(function (index) {
                    var f = this,
                        v = dataRow[index];
                    if (requiresFormatting && f.DataFormatString && f.FormatOnClient) {
                        if (f.Type.match(/^Date/) && typeof (v) == 'string')
                            v = Date.fromUTCString(v);
                        v = f.format(v);
                    }
                    newRow[f.Name] = v;
                });
            });
            if (this._requiresPivot) {
                r.Pivots = {};
                $(result.Pivots).each(function () {
                    r.Pivots[this.Name] = this;
                });
            }
            if (args.includeRawResponse || batchMode)
                r.rawResponse = result;
            if (batchMode)
                return r;
            resolve(r);
            cleanup();
        };

        if (args.response) {
            dataView._onGetPageComplete(args.response)
            return;
        }
        dataView._pendingExecuteComplete = {};
        dataView._onExecuteComplete = function (result) {
            var batchMode = !!args.batch;
            if (!batchMode)
                done();
            var r = result ?
                {
                    rowsAffected: result.RowsAffected,
                    canceled: result.Canceled,
                    clientScript: result.ClientScript,
                    navigateUrl: result.NavigateUrl,
                    errors: result.Errors
                } :
                {},
                obj = r[args.controller] = {};

            $(args.values).each(function () {
                var v = this;
                obj[v.name || v.field] = typeof v.newValue == 'undefined' ? (typeof v.oldValue == 'undefined' ? v.value : v.oldValue) : v.newValue;
            });

            if (result)
                $(result.Values).each(function () {
                    obj[this.Name] = this.Value;
                });
            //if (args.includeRawResponse || batchMode)
            r.rawResponse = result;
            if (batchMode)
                return r;
            resolve(r);
            cleanup();
        }

        if (args.batch) {
            dataView._busy(true);
            var selectBatch = [],
                executeBatch = [];
            $(args.batch).each(function () {
                var r = this,
                    batchRequest,
                    select = !r.command || r.command == 'Select';
                r.skipInvoke = true;
                if (select) {
                    if (r.pageIndex == null)
                        r.pageIndex = 0;
                    if (r.pageSize == null)
                        r.pageSize = 100;
                }
                batchRequest = _app.execute(r);
                if (select) {
                    batchRequest.Controller = r.controller;
                    batchRequest.View = r.view;
                    selectBatch.push(batchRequest);
                }
                else
                    executeBatch.push(batchRequest);
            });
            if (selectBatch.length)
                dataView._invoke('GetPageList', { requests: selectBatch }, function (result) {
                    done();
                    if (args.success) {
                        $(result).each(function (index) {
                            var r = this;
                            result[index] = dataView._onGetPageComplete(r);
                        });
                        args.success(result);
                    }
                    cleanup();
                }, null);
            if (executeBatch.length)
                dataView._invoke('ExecuteList', { requests: executeBatch }, function (result) {
                    done();
                    if (args.success) {
                        $(result).each(function (index) {
                            var r = this;
                            args.controller = executeBatch[index].Controller;
                            result[index] = dataView._onExecuteComplete(r);
                        });
                        args.success(result);
                    }
                    cleanup();
                }, null);
            return;
        }

        dataView._collectFieldValues = function () {
            var values = [];
            $(args.values).each(function () {
                var v = this,
                    oldValue = _app.stringifyDate(typeof v.oldValue == 'undefined' ? v.value : v.oldValue),
                    newValue = _app.stringifyDate(typeof v.newValue == 'undefined' ? oldValue : v.newValue);
                values.push({
                    Name: v.name || v.field,
                    NewValue: newValue,
                    OldValue: oldValue,
                    Modified: typeof v.modified != 'undefined' ? v.modified == true : oldValue != newValue,
                    ReadOnly: v.readOnly == true
                });
            });
            return values;
        }

        dataView._validateFieldValues = function () {
            return true;
        }
        if (args.pageIndex != null)
            dataView._pageIndex = args.pageIndex;
        if (args.pageSize == null)
            args.pageSize = 100;
        if (args.sort)
            args.sortExpression = args.sort;
        dataView._pageSize = args.pageSize;
        if (args.selectedValues)
            dataView.SelectedValues = args.selectedValues;
        if (args.selectedKeys) {
            dataView.multiSelect(true);
            dataView.set_selectedValue(args.selectedKeys.join(';'));
        }
        if (args.lastCommand)
            dataView.set_lastCommandName(args.lastCommand);
        if (args.lastCommandArgument)
            dataView.set_lastCommandArgument(args.lastCommandArgument);
        if (requiresNewRow)
            api.inserting = true;
        args._init = true;
        dataView.search(args);
        if (args._filter)
            dataView._filter = args._filter;
        dataView._filterDetailsText = args.filterDetails;
        if (!args.command)
            args.command = 'Select';
        if (args.command == 'Pivot') {
            args.command = 'Select';
            dataView._requiresPivot = true;
            if (args.pivotDefinitions) {
                for (fieldName in args.pivotDefinitions) {
                    if (args.pivotDefinitions[fieldName].length > 0)
                        dataView._pivotDefinitions += ';' + fieldName + "=" + args.pivotDefinitions[fieldName].join();
                }
            }
        }
        if (args.requiresRowCount)
            dataView._requiresRowCount = true;
        dataView._lookupContext = args.lookupContext;
        if (args.distinct)
            dataView._distinctValues = true;
        var skipInvoke = args.skipInvoke;
        if (skipInvoke)
            dataView._skipInvoke = true;
        if (args.command == 'Select')
            dataView._loadPage();
        else
            dataView.executeCommand({ commandName: args.command, commandArgument: args.argument || '', causesValidation: false });
        if (skipInvoke) {
            cleanup();
            return dataView._invokeArgs;
        }
        return deferred.promise();
    }

    _app.data = function (args) {
        var controller = args.d.Controller,
            id = args.id;
        _app.execute({
            format: args.format,
            response: args.d,
            controller: controller,
            success: function (result) {
                var dataObj = result[controller];
                delete result[controller];
                result.controller = controller;
                dataObj._metadata = result;
                dataObj._metadata.id = id;
                _app.data[id] = dataObj;
                var list = _app.data.objects;
                if (!list)
                    list = _app.data.objects = [];
                list.push(dataObj)
            }
        })
    }

    _app.dataBind = function (content, context) {
        if (_app.data.objects) {
            content.find('[data-control]').addBack('[data-control]').each(function () {
                var control = $(this),
                    processed = control.data('processed');

                if (!processed) {
                    control.data('processed', true);
                    var type = control.data('control'),
                        source = control.data('source'),
                        fieldName = control.data('field'),
                        topContext = context[context.length - 1];

                    if (type == 'form') {
                        var newContext = context.concat([_app.data[source]]);
                        control.children().each(function () {
                            _app.dataBind($(this), newContext);
                        });
                    }
                    else if (type == "repeater") {
                        if (source)
                            topContext = _app.data[source];
                        control.removeAttr('data-control data-source');
                        var template = control[0].innerHTML;
                        control.empty();
                        $(topContext).each(function () {
                            var newContext = context.slice(0),
                                element = $(template).appendTo(control);
                            newContext.push(this);
                            _app.dataBind(element, newContext);
                        });
                        // repeat control
                    }
                    else if (fieldName) {
                        switch (type) {
                            case 'label':
                                // find metadata
                                var i, it;
                                for (i = context.length; i--; i >= 0) {
                                    it = context[i];
                                    if (it._metadata) {
                                        $(it._metadata.fields).each(function () {
                                            if (this.Name == fieldName) {
                                                control.text(this.Label);
                                                return false;
                                            }
                                        });
                                        break;
                                    }
                                }
                                break;
                            case 'field':
                                if ($.isArray(topContext)) {
                                    topContext = topContext[0];
                                }
                                var val = topContext && topContext[fieldName];
                                if (val == null)
                                    control.text('');
                                else
                                    control.text(val);
                                break;
                        }
                    }
                }
            });
        }
    }

    findDataView = _app.findDataView = function (id) {
        return _app.find(id, 'Id');
    }

    _app.find = function (id, propertyName) {
        var cid,
            i, c, searchById = arguments.length == 1 || propertyName == 'Id',
            application = Sys.Application,
            list;
        if (!id)
            return null;
        if (searchById) {
            c = application._components[id];
            if (propertyName == 'Id')
                return c;
        }
        if (!c) {
            list = application.getComponents();
            cid = '_' + id;
            for (i = 0; i < list.length; i++) {
                c = list[i];
                if (propertyName) {
                    if (_app.isInstanceOfType(c) && ((propertyName == 'Controller' && c._controller == id) || (propertyName == 'Tag' && c.get_isTagged(id))))
                        return c;
                }
                else
                    if (_app.isInstanceOfType(c) && c._id.endsWith(cid))
                        return c;
            }
            if (searchById)
                c = _app.find(id, 'Controller');
        }
        return c;
    }

    _app.confirm = function (message, trueCallback, falseCallback) {
        var promise = $.Deferred();
        if (window.confirm(message)) {
            if (trueCallback)
                trueCallback();
            else
                promise.resolve();
        }
        else if (falseCallback)
            falseCallback();
        else
            promise.reject();
        return promise;
    }

    _app.alert = function (message, callback) {
        var promise = $.Deferred();
        window.alert(message);
        if (callback)
            callback();
        else
            promise.resolve();
        return promise;
    }

    _app.eval = function (text) {
        if (text) {
            var m = text.match(_jsExpRegex);
            while (m) {
                text = text.substring(0, m.index) + eval(m[1]) + text.substring(m.index + m[0].length);
                m = text.match(_jsExpRegex);
            }
        }
        return text;
    }

    _app.showModal = function (parent/*anchor*/, controller, view, startCommandName, startCommandArgument, baseUrl, servicePath, filter, properties) {
        var parentIsDataView = _app.isInstanceOfType(parent);
        var anchor = parent;
        if (!_touch) {
            if (parentIsDataView && parent._container)
                anchor = parent._container.getElementsByTagName('a')[0];
            if (!anchor) {
                var links = document.getElementsByTagName('a', 'input', 'button');
                for (var i = links.length - 1; i >= 0; i--) {
                    if (links[i].tabIndex >= 0) {
                        anchor = links[i];
                        break;
                    }
                }
            }
            if (anchor == null) {
                alert('Cannot find an anchor for a modal popup.');
                return;
            }
            closeHoverMonitorInstance();
        }
        if (_touch) {
            if (_touch.busy())
                return;
            _touch.modalDataView();
        }
        if (!baseUrl) baseUrl = _app._baseUrl;
        if (!servicePath) servicePath = _app._servicePath;
        var placeholder = this._placeholder = document.createElement('div'),
            loweredController = controller.toLowerCase().replace(/\_+/g, '-'),
            id = loweredController,
            instanceIndex = 1;
        while (Sys.Application._components[id])
            id = loweredController + (instanceIndex++);

        placeholder.id = String.format('{0}_{1}_Placeholder{2}', controller, view, Sys.Application.getComponents().length);
        if (parentIsDataView)
            parent._appendModalPanel(placeholder);
        else
            document.body.appendChild(placeholder);
        placeholder.className = 'ModalPlaceholder FixedDialog EmptyModalDialog';
        var survey,
            params = {
                id: id/*controller + '_ModalDataView' + applicationComponentCount*/, baseUrl: baseUrl, servicePath: servicePath,
                controller: controller, viewId: view, showActionBar: false, modalAnchor: anchor, startCommandName: startCommandName, startCommandArgument: startCommandArgument,
                exitModalStateCommands: ['Cancel'], externalFilter: filter
            };

        if (properties) {
            if (!properties.useCase) {
                properties.useCase = _app._defaultUseCase;
                _app._defaultUseCase = null;
            }
            params.filter = properties.filter;
            params.ditto = properties.ditto;
            params.lastViewId = properties.lastViewId;
            //params.transaction = properties.transaction;
            params.filterSource = properties.filterSource;
            params.filterFields = properties.filterFields;
            params.confirmContext = properties.confirmContext;
            params.showSearchBar = properties.showSearchBar;
            params.useCase = properties.useCase;
            params.tag = properties.tag;
            params.showActionButtons = properties.showActionButtons;
            survey = params.survey = properties.survey;
        }
        var dataView = $create(Web.DataView, params, null, null, placeholder),
            dataText = parentIsDataView ? parent._dataText : null;
        if (parentIsDataView) {
            var parentId = parent._id,
                parentExternalFilter = parent._externalFilter;

            if (_touch && _touch.pageInfo(parent._id).deleted) {
                parentId = parent._parentDataViewId;
                if (parentExternalFilter && parentExternalFilter.length)
                    dataView._externalFilter = parentExternalFilter;
            }
            dataView._parentDataViewId = parentId;
            dataView._hasDetails = parent._hasDetails;
        }
        if (_touch && dataText) {
            var pageInfo = _touch.pageInfo(dataView._id);
            pageInfo.headerText = dataText;
            pageInfo.headerTextLocked = true;

        }
        if (_touch)
            _touch._dataText = null;

        if (survey) {
            if (survey.text2 && typeof survey.text2 == 'function')
                survey.text2 = survey.text2.call(dataView);
            if (survey.text && typeof survey.text == 'function')
                survey.text = survey.text.call(dataView);
            if (!survey.text && survey.text2) {
                survey.text = survey.text;
                survey.text2 = null;
            }
            if (survey.text2) {
                if (_touch)
                    survey.text = [survey.text, survey.text2];
                else
                    survey.text += ' - ' + survey.text2;
            }
            else
                survey.text = survey.text;

            if (survey.description) {
                dataView.set_showDescription(true);
                dataView.set_description(survey.description);
            }
            survey.compiled = function (result) {
                if (_touch) {
                    survey.result = result;
                    _touch.modalDataView(params.id, true);
                }
                else
                    dataView._onGetPageComplete(result);
            };
            _app.survey('compile', survey)
        }
        else
            if (_touch)
                _touch.modalDataView(params.id, true);
        return dataView;
    }

    _app.alert = function (message, callback) {
        alert(message);
        if (callback)
            callback();
    }

    //_app.isSaaS = function () {
    //    var that = this;
    //    if (that._isSaaS == null) {
    //        that._isSaaS = false;
    //        $('script').each(function () {
    //            if (this.src.match(/\/factory(.*?)\.js/))
    //                that._isSaaS = true;
    //        });
    //    }
    //    return that._isSaaS;
    //}

    _app._resizeInterval = null;
    _app._resizing = false;
    _app._resized = false;
    _app._customInputElements = [];

    _app.contentFrameworks = {
        bootstrap: {
            hasSelectors: [
                { name: 'navbar-fixed-top', selector: '.navbar-fixed-top' },
                { name: 'navbar-fixed-bottom', selector: '.navbar-fixed-bottom' },
                { name: 'navbar-static-top', selector: '.navbar-static-top' },
                { name: 'footer', selector: 'footer' }
            ],
            footer: { selector: 'footer,.footer', content: { html: '<footer><div class="container"></div></footer>', selector: '.container' } },
            fixedTop: { selector: '.navbar-fixed-top' },
            fixedBottom: { selector: '.navbar-fixed-bottom' }
        }
    };

    _app.filterDef = function (filterDefs, func) {
        if (func.endsWith('$')) func = func.substring(0, func.length - 1);
        for (var i = 0; i < filterDefs.length; i++) {
            var fd = filterDefs[i];
            if (fd) {
                if (fd.List) {
                    var result = _app.filterDef(fd.List, func);
                    if (result) return result;
                }
                else if (fd.Function == func)
                    return fd;
            }
        }
        return null;
    };


    _app._invoke = function (methodName, args, success, error) {
        var placeholder = $('<p>'),
            dataView = $create(Web.DataView, { servicePath: __servicePath, baseUrl: __baseUrl, useCase: '$app' }, null, null, placeholder.get(0));
        dataView._busy(true);
        dataView._invoke(methodName, args, function (result) {
            dataView._busy(false);
            if (success)
                success(result);
            dataView.dispose();
            placeholder.remove();
        }, null, error);
    };

    _app.callWithFeedback = function (link, callback) {
        if (_touch)
            _touch.callWithFeedback(link, callback);
        else
            callback();
    }

    _app.sizeToText = sizeToText;

    _app.userName = function () {
        return __settings.appInfo.split('|')[1];
    }

    _app.userId = function () {
        return __settings.appInfo.split('|')[2];
    }

    _app.loggedIn = function () {
        return !!_app.userName();
    }

    _app.login = function (username, password, createPersistentCookie, success, error) {
        _app._invoke('Login', { username: username, password: password, createPersistentCookie: _app.AccountManager.enabled() ? false : createPersistentCookie }, function (result) {
            if (result && result != 'false') {
                if (result != "true") {
                    if (!createPersistentCookie)
                        result.Token = null;
                    _app.AccountManager.set(result);
                    //if (createPersistentCookie)
                    //    _app.AccountManager.set(result);
                    //else
                    //    _app.AccountManager.remove(username, true);
                }
                if (success)
                    success();
            }
            else
                if (error)
                    error();
        });
    }

    _app.switchUser = function (user, success, error) {
        _app._invoke(
            'Login',
            { username: user.UserName, password: 'token:' + user.Token, createPersistentCookie: false },
            function (result) {
                if (result) {
                    if (result != "true") {
                        if (user.Handler)
                            result.Handler = user.Handler;
                        _app.AccountManager.set(result);
                    }
                    if (success)
                        success(result);
                }
                else
                    if (error)
                        error();
            });
    }

    _app.logout = function (callback) {
        _app.AccountManager.remove(_app.userName());
        _app._invoke('Logout', {}, function () {
            if (callback)
                callback();
        });
    }

    _app.roles = function (callback) {
        _app._invoke('Roles', {}, function (result) {
            if (callback)
                callback(result);
        });
    }


    _app.configureFramework = function (framework, pageContent, callback) {
        $(_app.contentFrameworks[framework]).each(function () {
            var fw = this;
            $(fw.hasSelectors).each(function () {
                if (pageContent.find(this.selector).length)
                    pageContent.addClass('has-' + this.name);
            });
            if (fw.footer && !pageContent.find(fw.footer.container).find(fw.footer.selector).length)
                $(fw.footer.content.html).appendTo(pageContent).find(fw.footer.content.selector).append($('#PageFooterBar,footer small').html());
            if (callback)
                callback(fw);
        });
    }

    _app.borderBox = function (elem) {
        var $elem = $(elem);
        //var borderTop = $elem.css('border-top-width');
        //var borderBottom = $elem.css('border-bottom-width');
        return { horizontal: $elem.outerWidth() - $elem.innerWidth(), vertical: $elem.outerHeight() - $elem.innerHeight() };
    }
    _app.paddingBox = function (elem) {
        var $elem = $(elem);
        return { horizontal: $elem.innerWidth() - $elem.width(), vertical: $elem.innerHeight() - $elem.height() };
    }
    _app.marginBox = function (elem) {
        var $elem = $(elem);
        return { horizontal: $elem.outerWidth(true) - $elem.outerWidth(), vertical: $elem.outerHeight(true) - $elem.outerHeight() };
    }

    _app.bounds = function (elem) {
        var $elem = $(elem);
        var offset = $elem.offset();
        return { x: offset.left, y: offset.top, width: $elem.outerWidth(), height: $elem.outerHeight() };
    }

    _app.scrolling = function () {
        return { x: $window.scrollLeft(), y: $window.scrollTop() };
    }

    _app.clientBounds = function () {
        return { width: $window.width(), height: $window.height() };
    }


    _window._body_hideLayoutContainers = function () {
        if (!_app._layoutContainers) return;
        for (var i = 0; i < _app._layoutContainers.length; i++) {
            var lc = _app._layoutContainers[i];
            if (lc.width != '100%')
                Sys.UI.DomElement.setVisible($get(lc.id), false);
        }
    }

    _window._body_resizeLayoutContainers = function () {
        var layoutContainers = _app._layoutContainers;
        if (!layoutContainers || layoutContainers.length == 0) return;
        var pc = $get('PageContent');
        if (!pc) return;
        //var bounds = $common.getBounds(pc);
        var bounds = _app.bounds(pc);
        var padding = _app.paddingBox(pc);
        //var padding2 = $common.getPaddingBox(pc);
        var border = _app.borderBox(pc);
        //var border2 = $common.getBorderBox(pc);
        var margin = _app.marginBox(pc); // $common.getMarginBox(pc);
        //var margin2 = $common.getMarginBox(pc);
        bounds.width -= padding.horizontal + border.horizontal + margin.horizontal;
        var rowIndex = layoutContainers[layoutContainers.length - 1].rowIndex;
        while (rowIndex >= layoutContainers[0].rowIndex) {
            var usedSpace = 0;
            for (var i = 0; i < layoutContainers.length; i++) {
                var lc = layoutContainers[i];
                if (lc.rowIndex == rowIndex && !isNullOrEmpty(lc.width)) {
                    var div = $get(lc.id);
                    if (div) {
                        var divPadding = _app.paddingBox(div); // $common.getPaddingBox(div);
                        var divBorder = _app.borderBox(div); // $common.getBorderBox(div);
                        var divMargin = _app.marginBox(div); // $common.getMarginBox(div);
                        var m = lc.width.match(/(\d+)(%|px|)/);
                        var divWidth = m[2] != '%' ? parseFloat(m[1]) : Math.floor(bounds.width * parseFloat(m[1]) / 100);
                        usedSpace += divWidth;
                        divWidth -= divPadding.horizontal + divBorder.horizontal + divMargin.horizontal
                        if (lc.width != '100%') {
                            div.style.width = divWidth + 'px';
                            Sys.UI.DomElement.setVisible(div, true);
                        }
                        else {
                            $(div).removeClass('LayoutContainer').addClass('RowContainer');
                        }
                    }
                }
            }
            if (usedSpace < bounds.width) {
                for (i = 0; i < layoutContainers.length; i++) {
                    lc = layoutContainers[i];
                    if (lc.rowIndex == rowIndex && isNullOrEmpty(lc.width)) {
                        div = $get(lc.id);
                        if (div) {
                            divPadding = _app.paddingBox(div); // $common.getPaddingBox(div);
                            divBorder = _app.borderBox(div); // $common.getBorderBox(div);
                            divMargin = _app.marginBox(div); // $common.getMarginBox(div);
                            divWidth = Math.floor((bounds.width - usedSpace) / lc.peersWithoutWidth);
                            divWidth -= divPadding.horizontal + divBorder.horizontal + divMargin.horizontal
                            if (divWidth < 1) divWidth = 1;
                            div.style.width = divWidth + 'px';
                            Sys.UI.DomElement.setVisible(div, true);
                        }
                    }
                }
            }
            rowIndex--;
        }
    }

    _window._body_keydown = function (e) {
        if (e.keyCode == Sys.UI.Key.enter && _app._focusedItemIndex != null) {
            var dv = $find(_app._focusedDataViewId);
            if (dv && dv._get_focusedCell())
                return;
            var elem = $get(_app._focusedDataViewId + '_Item' + _app._focusedItemIndex);
            if (elem && elem.tagName == 'INPUT' && elem.type == 'text' && elem == e.target) {
                e.preventDefault();
                e.stopPropagation();
                dv = $find(_app._focusedDataViewId);
                if (dv) dv._valueChanged(_app._focusedItemIndex);
            }
        }
    }

    _window._body_resize = function () {
        if ($getSideBar())
            $('body,.MembershipBarPlaceholder').width('');
        if (_app._resizeInterval)
            clearInterval(_app._resizeInterval);
        if (!_app._resizing && !_app._resized) {
            _app._resizeInterval = setInterval(function () {
                _body_hideLayoutContainers();
                _body_resizeLayoutContainers();
                _body_performResize();
            }, 200);
        }
        else
            $closeHovers();
        _app._resized = false;
    }

    _window._body_scroll = function () {
        var sideBar = $getSideBar();
        if (!sideBar) return;
        var scrolling = _app.scrolling(); // { x: $(window).scrollLeft(), y: $(window).scrollTop() };// $common.getScrolling();
        var clientBounds = $common.getClientBounds();
        var bounds = $common.getBounds(sideBar);
        if (sideBar._originalTop == null)
            sideBar._originalTop = bounds.y;
        var originalTop = sideBar._originalTop;
        var deltaY = 0;
        if (_app.MessageBar) {
            var mbb = $common.getBounds(_app.MessageBar._element);
            originalTop += deltaY = mbb.height;
        }
        if (scrolling.y > sideBar._originalTop && bounds.height + 4 <= clientBounds.height && scrolling.x == 0) {
            sideBar.style.width = bounds.width + 'px';
            if (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version <= 6) {
                sideBar.style.top = (4 + scrolling.y + deltaY) + 'px';
                sideBar.style.position = 'absolute';
            }
            else {
                sideBar.style.top = 4 + deltaY + 'px';
                sideBar.style.position = 'fixed';
            }
        }
        else {
            sideBar.style.top = '';
            sideBar.style.width = '';
            sideBar.style.position = '';
        }
        $('body,.MembershipBarPlaceholder').width($('table#PageBody').outerWidth());
    }

    _window._body_createPageContext = function (persist) {
        var pc = $get('PageContent');
        if (!pc) return;
        var b = $common.getBounds(pc);
        var pb = $common.getPaddingBox(pc);
        var bb = $common.getBorderBox(pc);
        var ctx = { 'height': b.height - pb.vertical - bb.vertical, 'scrolling': _app.scrolling()/* $common.getScrolling()*/ };
        if (persist != false) _app._pageContext = ctx;
        return ctx;
    }

    _window._body_performResize = function () {
        if (_app._resizeInterval) clearInterval(_app._resizeInterval);
        _app._resizeInterval = null;
        $closeHovers();
        var pc = $get('PageContent');
        if (!pc || _touch) return;

        var cb = $common.getClientBounds();
        var clientBounds = $common.getClientBounds();
        if (_app.MessageBar) {
            var messageBarElement = _app.MessageBar._element;
            if ($common.getVisible(messageBarElement)) {

                var mbeb = $common.getPaddingBox(messageBarElement);
                messageBarElement.style.width = (clientBounds.width - mbeb.horizontal) + 'px';
                var messageContainer = messageBarElement.childNodes[0];
                messageContainer.style.height = '';
                var panelBounds = $common.getBounds(messageContainer);
                var maxMessageHeight = Math.ceil(cb.height * 0.15)
                if (panelBounds.height > maxMessageHeight) {
                    messageContainer.style.height = maxMessageHeight + 'px';
                    messageContainer.style.overflow = 'auto';
                }
                mbeb = Sys.UI.DomElement.getBounds(messageBarElement);
                var bodyTop = _app.OriginalBodyTopOffset + mbeb.height;
                document.body.style.paddingTop = bodyTop + 'px';
                var loginDialog = $get("Membership_Login");
                if (loginDialog) loginDialog.style.marginTop = (bodyTop) + 'px';
            }
        }
        _app._resizing = true;
        var pageContext = _app._pageContext;
        if (pageContext == null)
            pageContext = _body_createPageContext(false);
        else
            _app._pageContext = null;
        var scrolling = _app.scrolling(); // $common.getScrolling();
        if (scrolling.y == 0) pc.style.height = '10px';
        _body_resizeLayoutContainers();
        //_body_scroll();
        if (typeof (__cothost) == 'undefined') {
            var bounds = _app.bounds(pc); // $common.getBounds(pc);
            var padding = _app.paddingBox(pc); // $common.getPaddingBox(pc);
            var border = _app.borderBox(pc); // $common.getBorderBox(pc);
            var pmb = Web.Menu.MainMenuElemId ? $get(Web.Menu.MainMenuElemId) : null;
            var pmbBorderBox = pmb ? $common.getBorderBox(pmb) : null;
            var pfc = $get('PageFooterContent');
            var pfb = $get('PageFooterBar');
            var newHeight = scrolling.y + cb.height - bounds.y - (pfb ? pfb.offsetHeight : 0) - (pfc ? pfc.offsetHeight : 0) - border.vertical - padding.vertical - (pmbBorderBox ? pmbBorderBox.vertical : 0);
            if (bounds.height < newHeight) {
                if (Sys.Browser.agent == Sys.Browser.Opera)
                    newHeight += border.vertical + padding.vertical;
            }
            if (pageContext.scrolling.y == 0 || _app._numberOfContainers < 2)
                pc.style.height = document.body.offsetHeight > cb.height ? '' : newHeight + 'px';
            else {
                pc.style.height = pageContext.height + 'px';
                scrollTo(0, pageContext.scrolling.y);
            }
        }
        _body_scroll();
        _app._resizing = false;
        _app._resized = true;
    }

    _app._activate = function (source, elementId, type) {
        var activatorRegex = new RegExp('^\\s*' + type + '\\s*\\|');
        var elem = $get(elementId);
        if (type == 'SideBarTask') {
            var lc = elem;
            while (lc && isNullOrEmpty(this.prototype.dataAttr(lc, 'flow')))
                lc = lc.parentNode;
            var peers = lc.getElementsByTagName('div');
            for (var i = 0; i < peers.length; i++) {
                var activator = this.prototype.dataAttr(peers[i], 'activator');
                if (!isNullOrEmpty(activator) && activatorRegex.exec(activator))
                    $(peers[i]).hide();
            }
        }
        Sys.UI.DomElement.setVisible(elem, type == 'SideBarTask' ? true : !Sys.UI.DomElement.getVisible(elem));
        elem._activated = true;
        if (type == 'SiteAction' && elem.childNodes[0].className != 'CloseSiteAction') {
            var closeLink = document.createElement('div');
            closeLink.className = 'CloseSiteAction';
            closeLink.innerHTML = String.format('<a href="javascript:" onclick="$app._activate(null,\'{0}\',\'SiteAction\')">{1}</a>', elementId, resourcesModalPopup.Close);
            elem.insertBefore(closeLink, elem.childNodes[0]);
        }
        if (Sys.UI.DomElement.getVisible(elem)) {
            var bounds = $common.getBounds(elem);
            var clientBounds = $common.getClientBounds();
            var scrolling = _app.scrolling(); // $common.getScrolling();
            if (bounds.y < scrolling.y || bounds.y > scrolling.y + clientBounds.height)
                elem.scrollIntoView(false);
        }
        if (source) {
            while (source && !Sys.UI.DomElement.containsCssClass(source, 'Task'))
                source = source.parentNode;
            for (i = 0; i < source.parentNode.childNodes.length; i++) {
                var peer = source.parentNode.childNodes[i];
                if (peer.className)
                    Sys.UI.DomElement.removeCssClass(peer, 'Selected');
            }
            Sys.UI.DomElement.addCssClass(source, 'Selected');
        }
        _body_performResize();
    }

    _app._partialUpdateBeginRequest = function (sender, args) {
        var r = args.get_request();
        var components = Sys.Application.getComponents();
        var controllers = [];
        for (var i = 0; i < components.length; i++) {
            var c = components[i];
            if (c._controller && c._viewId && _app.isInstanceOfType(c)) {
                var tag = c.get_tag();
                if (!isNullOrEmpty(tag)) {
                    Array.add(controllers, tag);
                    Array.add(controllers, c.get_selectedKey());
                }
            }
        }
        var s = _serializer.serialize(controllers);
        r.set_body(r.get_body() + '&' + encodeURIComponent('__WEB_DATAVIEWSTATE') + '=' + encodeURIComponent(s));
    }

    function findSelectedMenuNode(nodes) {
        var result = null;
        $(nodes).each(function () {
            var n = this;
            if (n.selected) {
                result = n;
                return false;
            }
            else if (n.children) {
                result = findSelectedMenuNode(n.children);
                if (result)
                    return false;
            }
        });
        return result;
    }

    function buildSiteMap(parent, nodes) {
        $(nodes).each(function () {
            var n = this,
                href = n.url,
                target,
                hrefParts,
                li = $('<li/>').appendTo(parent),
                a;
            if (href) {
                hrefParts = href.match(/^(_\w+):(.+)$/);
                if (hrefParts) {
                    href = hrefParts[2];
                    target = hrefParts[1];
                }
                a = $('<a/>').attr({ href: href, title: n.description, target: target });
            }
            else
                a = $('<span/>');
            a.appendTo(li).text(n.title);
            if (n.children) {
                buildSiteMap($('<ul/>').appendTo(li), n.children);
            }
        });
    }

    _app._contentFactories = {
        'site-map': function (container) {
            var startFromCurrentNode = container.attr('data-start-from-current-node') == 'true',
                _menu = Web.Menu,
                nodes = _menu.Nodes[_menu.MainMenuId],
                currentNode = findSelectedMenuNode(nodes),
                ul = $('<ul class="SiteMapPlaceholder"/>').appendTo(container);
            buildSiteMap(ul, startFromCurrentNode ? currentNode.children : nodes);
        }
    }

    _app._load = function () {
        if (_app._loaded) return;
        if (Sys.WebForms && Sys.WebForms.PageRequestManager._instance) Sys.WebForms.PageRequestManager._instance.add_beginRequest(_app._partialUpdateBeginRequest);
        _app._loaded = true;
        updateACT();
        if (typeof __baseUrl == 'undefined')
            __baseUrl = '../'
        $('#PageHeaderSideBar .PageLogo').attr('src', __baseUrl + 'app_themes/_shared/placeholder.gif');
        _app.supportsPlaceholder = 'placeholder' in document.createElement('input');
        var pc = $get('PageContent'),
            hasContent,
            contentPages = [],
            body = $('body');
        if (pc) {
            body.find('div[data-app-role="page"]').each(function () {
                if ($(this).attr('data-content-framework'))
                    contentPages.push($(this).attr('id'));
            }).each(function () {
                var page = $(this),
                    pageId = page.attr('id'),
                    hash = location.search.match(/_hash=(\w+)/),
                    framework = page.attr('data-content-framework'),
                    content = page.find('> div[data-role="content"]');
                if (!content.length) {
                    content = $('<div data-role="content"></div>');
                    page.contents().appendTo(content);
                    content.appendTo(page);
                }
                if (framework)
                    if (!hasContent && (!hash || hash[1] == pageId)) {
                        var table = $(pc).closest('table');
                        page.insertAfter(table);
                        body.addClass('app-min-lg').addClass('app-theme-' + __settings.ui.theme.accent.toLowerCase());
                        hasContent = true;
                        content = page.find('> div[data-role="content"]').addClass('app-bootstrap app-content-desktop');
                        _app.configureFramework(framework, content);
                        page.css({ position: 'absolute', 'top': table.offset().top, width: '100%' });
                        table.hide();
                        page.find('a').each(function () {
                            var link = $(this),
                                href = link.attr('href'),
                                pageId = href && href.startsWith('#') && href.length > 1 && href.substring(1);
                            if (pageId && Array.indexOf(contentPages, pageId) != -1)
                                link.attr('href', '?_hash=' + pageId);
                        });
                        $('#PageFooterBar').hide();
                        return;
                    }
                page.remove();
            });
            $(pc).find('div[data-role="placeholder"]').each(function () {
                var placeholder = $(this),
                    factory = _app._contentFactories[placeholder.attr('data-placeholder')];
                if (factory)
                    factory(placeholder);
            });
            //userPages.remove();
            var divs = document.body.getElementsByTagName('div');
            for (var i = 0; i < divs.length; i++) {
                if (divs[i].className.match(/Loading/)) {
                    Sys.UI.DomElement.removeCssClass(divs[i], 'Loading');
                    break;
                }
            }
            divs = pc.getElementsByTagName('div');
            var layoutContainers = [];
            var rowIndex = 0;
            var hasActivators = false;
            var hasSideBarActivators = false;
            var sb = null;
            var siteActions = [];
            _app._numberOfContainers = 0;
            var dataAttr = _app.prototype.dataAttr;
            for (i = 0; i < divs.length; i++) {
                var div = divs[i];
                var width = dataAttr(div, 'width');
                var flow = dataAttr(div, 'flow');
                if (!isNullOrEmpty(width) || !isNullOrEmpty(flow)) {
                    if ($(div).find('div[data-flow]').length)
                        continue;
                    if (flow != 'NewColumn' && flow != 'column') {
                        div.style.clear = 'left';
                        rowIndex++;
                    }
                    if (isNullOrEmpty(div.id))
                        div.id = "_lc" + layoutContainers.length;
                    Sys.UI.DomElement.addCssClass(div, 'LayoutContainer');
                    _app._numberOfContainers++;
                    if (width != '100%')
                        div.style.overflow = 'hidden';
                    Array.add(layoutContainers, { 'id': div.id, 'width': width, 'rowIndex': rowIndex, 'peersWithoutWidth': 0 });
                    var childDivs = div.getElementsByTagName('div');
                    var divsWithActivator = [];
                    for (var j = 0; j < childDivs.length; j++) {
                        var childDiv = childDivs[j];
                        var activator = dataAttr(childDiv, 'activator');
                        if (!isNullOrEmpty(activator) && !isBlank(childDiv.innerHTML)) {
                            childDiv.id = isNullOrEmpty(childDiv.id) ? div.id + '$a' + j : childDiv.id
                            var da = { 'elem': childDiv, 'activator': _app.eval(activator).split('|'), 'id': childDiv.id };
                            da.activator[0] = da.activator[0].trim();
                            if (da.activator.length == 1) da.activator[1] = j.toString();
                            Array.add(divsWithActivator, da);
                        }
                    }
                    j = 1;
                    while (j < divsWithActivator.length) {
                        da = divsWithActivator[j];
                        for (var k = 0; k < j; k++) {
                            var da2 = divsWithActivator[k];
                            if (da2.activator[0] == da.activator[0] & da2.activator[1] == da.activator[1]) {
                                while (da.elem.childNodes.length > 0)
                                    da2.elem.appendChild(da.elem.childNodes[0]);
                                delete da.elem;
                                Array.removeAt(divsWithActivator, j);
                                da = null;
                                break;
                            }
                        }
                        if (da) j++;
                    }
                    if (divsWithActivator.length > 0) {
                        hasActivators = true;
                        var nodes = [];
                        var firstSideBarActivator = true;
                        for (j = 0; j < divsWithActivator.length; j++) {
                            da = divsWithActivator[j];
                            if (da.activator[0] == 'Tab') {
                                if (nodes.length == 0) {
                                    var menuBar = document.createElement('div');
                                    menuBar.className = 'TabBar';
                                    div.insertBefore(menuBar, da.elem);
                                }
                                var n = { 'title': da.activator[1], 'elementId': da.id, 'selected': nodes.length == 0, 'description': dataAttr(da.elem, 'description'), 'hidden': dataAttr(da.elem, 'hidden') };
                                Array.add(nodes, n);
                                Sys.UI.DomElement.setVisible(da.elem, n.selected);
                                da.elem._activated = true;
                                Sys.UI.DomElement.addCssClass(da.elem, 'TabBody TabContainer');
                            }
                            else if (da.activator[0] == 'SideBarTask') {
                                if (!sb) {
                                    sb = new Sys.StringBuilder();
                                    sb.appendFormat('<div class="TaskBox TaskList"><div class="Inner"><div class="Header">{0}</div>', resources.Menu.Tasks);
                                }
                                da.elem._activated = firstSideBarActivator;
                                if (firstSideBarActivator) firstSideBarActivator = false;
                                Sys.UI.DomElement.setVisible(da.elem, da.elem._activated);
                                sb.appendFormat('<div class="Task{1}"{4}><a href="javascript:" onclick="$app._activate(this,\'{2}\',\'SideBarTask\');return false;" title=\'{3}\'>{0}</a></div>', da.activator[1], !hasSideBarActivators ? ' Selected' : '', da.id,
                                    _app.htmlAttributeEncode(dataAttr(da.elem, 'description')), dataAttr(da.elem, 'hidden') == 'true' ? ' style="display:none"' : '');
                                hasSideBarActivators = true;
                            }
                            else if (da.activator[0] == 'SiteAction' && Web.Menu.get_siteActionsFamily()) {
                                var item = new Web.Item(Web.Menu.get_siteActionsFamily(), da.activator[1], dataAttr(da.elem, 'description'));
                                item.set_cssClass(dataAttr(da.elem, 'cssClass'));
                                item.set_script('$app._activate(null,"{0}","SiteAction")', da.id);
                                Array.add(siteActions, item);
                                Sys.UI.DomElement.setVisible(da.elem, false);
                            }
                            else {
                                Sys.UI.DomElement.setVisible(da.elem, false);
                            }
                            da.elem = null;
                        }
                        if (nodes.length > 0) {
                            $create(Web.Menu, { 'id': div.id + '$ActivatorMenu', 'nodes': nodes }, null, null, menuBar);
                            if (nodes.length < 2 && i == 0)
                                Sys.UI.DomElement.addCssClass(menuBar, 'EmptyTabBar');
                        }
                    }
                }
            }
            if (hasActivators)
                _app._performDelayedLoading();
            if (hasSideBarActivators && sb) {
                var sideBar = $getSideBar();
                if (sideBar) {
                    sb.append('</div></div>');
                    var tasksBox = document.createElement('div');
                    tasksBox.innerHTML = sb.toString();
                    sb.clear();
                    sideBar.insertBefore(tasksBox, sideBar.childNodes[0]);
                    sideBar._hasActivators = true;
                }
            }
            if (siteActions.length > 0)
                Web.Menu.set_siteActions(siteActions);
            var ri = rowIndex;
            while (layoutContainers.length > 0 && ri >= layoutContainers[0].rowIndex) {
                var containersWithoutWidth = 0;
                var peerCount = 0;
                for (i = 0; i < layoutContainers.length; i++) {
                    lc = layoutContainers[i];
                    if (lc.rowIndex == ri) {
                        peerCount++;
                        if (isNullOrEmpty(lc.width))
                            containersWithoutWidth++;
                    }
                }
                for (i = 0; i < layoutContainers.length; i++) {
                    lc = layoutContainers[i];
                    if (lc.rowIndex == ri) {
                        lc.peersWithoutWidth = containersWithoutWidth;
                        if (peerCount == 1 && isNullOrEmpty(lc.width))
                            lc.width = '100%';
                    }
                }
                ri--;
            }
            _app._layoutContainers = layoutContainers;
            _body_performResize();
            $addHandler(window, 'resize', _body_resize);
            $addHandler(window, 'scroll', _body_scroll);
        }
        _app._startDelayedLoading();
        $addHandler(document.body, 'keydown', _body_keydown);
    }

    _app._unload = function () {
        if (this._state) {
            Array.clear(this._state);
            delete this._state;
        }
        if (_app._delayedLoadingTimer) {
            clearInterval(_app._delayedLoadingTimer);
            Array.clear(_app._delayedLoadingViews);
            delete _app._delayedLoadingViews;
        }
        if ($get('PageContent')) {
            $removeHandler(window, 'resize', _body_resize);
            $removeHandler(window, 'scroll', _body_scroll);
        }
        $removeHandler(document.body, 'keydown', _body_keydown);
    }

    _app._startDelayedLoading = function () {
        if (_app._delayedLoadingViews.length > 0 && !_app._delayedLoadingTimer)
            _app._delayedLoadingTimer = setInterval(function () {
                _app._performDelayedLoading();
            }, 1000);
    }

    _app._updateBatchSelectStatus = function (cb, isForm) {
        var targetClass = isForm ? 'Item' : 'Cell';
        var elem = cb.parentNode;
        while (elem != null && !Sys.UI.DomElement.containsCssClass(elem, targetClass)) elem = elem.parentNode;
        if (elem) {
            if (cb.checked)
                Sys.UI.DomElement.addCssClass(elem, 'BatchEditFrame');
            else
                Sys.UI.DomElement.removeCssClass(elem, 'BatchEditFrame');
        }
    }

    _app.highlightFilterValues = function (elem, active, className) {
        while (elem && elem.tagName != 'TABLE')
            elem = elem.parentNode;
        if (elem)
            if (active && !elem.className.match(className))
                Sys.UI.DomElement.addCssClass(elem, className);
            else if (!active && elem.className.match(className))
                Sys.UI.DomElement.removeCssClass(elem, className);
    }

    _app.get_commandLine = function () {
        var commandLine = _app._commandLine;
        if (!commandLine) {
            if (typeof __dacl != 'undefined')
                commandLine = __dacl;
            if (!commandLine) {
                commandLine = typeof Web.Membership != 'undefined' && Web.Membership._instance ? Web.Membership._instance.get_commandLine() : null;
                commandLine = !commandLine ? location.href : location.pathname + '?' + commandLine;
            }
            _app._commandLine = _app.unanchor(commandLine);
        }
        return commandLine;
    }

    /*
    _app._run2 = function (baseUrl, servicePath, authenticated) {
    var pageContent = $('#PageContent, div[data-role="page"]');
    if (!authenticated) {
    var placeholderSupported = 'placeholder' in document.createElement('input');
    var login = $('<div id="appfactory-login"><div class="user-name"><label for="username">User Name:</label><input type="text" id="username" placeholder="User Name"/></div><div class="password"><label for="password">Password:</label><input type="password" id="password" placeholder="Password"/></div><div class="login"><button>Login</button><div></div>').insertBefore(pageContent);
    login.find('button').click(function (e) {
    e.preventDefault();
    var username = login.find('#username');
    var password = login.find('#password');
    $.ajax({
    url: baseUrl + '/appservices/_authenticate',
    type: 'GET',
    cache: false,
    data: { args: Sys.Serialization.Java3.serialize([username.val(), password.val()]) },
    dataType: 'jsonp',
    success: function (result) {
    if (result)
    location.replace(location.href);
    else
    username.focus();
    },
    error: function (e) {
    alert('error');
    }
    });
    });
    if (placeholderSupported)
    login.find('label').remove();
    pageContent.hide();
    return;
    }
    $('div[data-controller]').each(function () {
    var extender = $(this);
    var controller = extender.attr('data-controller');
    if (controller) {
    var id = extender.attr('id');
    if (!id)
    id = controller;
    // sample baseUrl - 'http://localhost:23920/Jsonp/'
    // sample servicePath - "../Services/DataControllerService.asmx"
    var args = { id: id, controller: controller, baseUrl: baseUrl, servicePath: servicePath, showSearchBar: true };
    var properties = [
    { name: 'view' },
    { name: 'pageSize', type: 'int' },
    { name: 'servicePath' },
    { name: 'filterSource' },
    { name: 'filterFields' },
    { name: 'autoHide', aliases: ['nothing', 'self', 'container'], values: [0, 1, 2] },
    { name: 'showSearchBar', type: 'bool' },
    { name: 'showModalForms', type: 'bool' }
    ];
    $(properties).each(function () {
    var v = extender.attr('data-' + this.name.replace(/([A-Z])/g, '-$1'));
    if (v) {
    if (this.aliases)
    v = this.values[Array.indexOf(this.aliases, v.toString().toLowerCase())];
    if (this.type == 'bool')
    v = v == 'true';
    if (this.type == 'int')
    v = parseInt(v);
    args[this.name] = v;
    }
    });
    $create(Web.DataView, args, null, null, this);
    }
    });
    }
    */

    _app._parse = function () {
        _touch = _app.touch;
        if (this._parsed) return;
        this._parsed = true;
        //var pageContent = $('#PageContent, div[data-role="page"]');
        //if (!authenticated) {
        //    var placeholderSupported = 'placeholder' in document.createElement('input');
        //    var login = $('<div id="appfactory-login"><div class="user-name"><label for="username">User Name:</label><input type="text" id="username" placeholder="User Name"/></div><div class="password"><label for="password">Password:</label><input type="password" id="password" placeholder="Password"/></div><div class="login"><button>Login</button><div></div>').insertBefore(pageContent);
        //    login.find('button').click(function (e) {
        //        e.preventDefault();
        //        var username = login.find('#username');
        //        var password = login.find('#password');
        //        $.ajax({
        //            url: baseUrl + '/appservices/_authenticate',
        //            type: 'GET',
        //            cache: false,
        //            data: { args: Sys.Serialization.JavaScriptSerializer.serialize([username.val(), password.val()]) },
        //            dataType: 'jsonp',
        //            success: function (result) {
        //                if (result)
        //                    location.replace(location.href);
        //                else
        //                    username.focus();
        //            },
        //            error: function (e) {
        //                alert('error');
        //            }
        //        });
        //    });
        //    if (placeholderSupported)
        //        login.find('label').remove();
        //    pageContent.hide();
        //    return;
        //}
        $('div[data-controller]').each(function () {
            var extender = $(this);
            var controller = extender.attr('data-controller');
            if (controller) {
                var id = extender.attr('id');
                if (!id)
                    id = controller;
                var args = { id: id, controller: controller, baseUrl: __baseUrl, servicePath: __servicePath, showSearchBar: true };
                var properties = [
                    { name: 'autoHide', aliases: ['nothing', 'self', 'container'], values: [0, 1, 2] },
                    { name: 'autoSelectFirstRow', type: 'bool' },
                    { name: 'autoHighlightFirstRow', type: 'bool' },
                    { name: 'filterSource' },
                    { name: 'filterFields' },
                    { name: 'pageSize', type: 'int' },
                    { name: 'refreshInterval', type: 'int' },
                    { name: 'searchByFirstLetter', type: 'bool', propName: 'showFirstLetters' },
                    { name: 'searchOnStart', type: 'bool' },
                    { name: 'selectionMode', aliases: ['single', 'multiple'], values: ['Single', 'Multiple'] },
                    { name: 'showActionBar', type: 'bool' },
                    { name: 'showActionButtons', aliases: ['', 'none', 'top', 'bottom', 'top-and-bottom'], values: ['Auto', 'None', 'Top', 'Bottom', 'TopAndBottom'] },
                    { name: 'showDetailsInListMode', type: 'bool' },
                    { name: 'showDescription', type: 'bool' },
                    { name: 'showInSummary', type: 'bool' },
                    { name: 'showModalForms', type: 'bool' },
                    { name: 'showQuickFind', type: 'bool' },
                    { name: 'showPageSize', type: 'bool' },
                    { name: 'showPager', aliases: ['none', 'top', 'bottom', 'top-and-bottom'], values: ['None', 'Top', 'Bottom', 'TopAndBottom'] },
                    { name: 'showRowNumber', type: 'bool' },
                    { name: 'showSearchBar', type: 'bool' },
                    { name: 'showViewSelector', type: 'bool' },
                    { name: 'startCommandName' },
                    { name: 'startCommandArgument' },
                    { name: 'summaryFieldCount', type: 'int' },
                    { name: 'tag' },
                    { name: 'tags' },
                    { name: 'view', propName: 'viewId' },
                    { name: 'visibleWhen' }
                ];
                $(properties).each(function () {
                    var prop = this,
                        v = extender.attr('data-' + prop.name.replace(/([A-Z])/g, '-$1'));
                    if (v) {
                        if (prop.aliases)
                            v = prop.values[Array.indexOf(prop.aliases, v.toString().toLowerCase())];
                        if (prop.type == 'bool')
                            v = v == 'true';
                        if (prop.type == 'int')
                            v = parseInteger(v);
                        args[prop.propName || prop.name] = v;
                    }
                });
                extender.attr('id', null);
                $create(Web.DataView, args, null, null, this);
            }
        });
    }

    _app.DetailsRegex = /\/Details\.aspx\?/i;
    _app.LocationRegex = /^(_.+?):(.+)$/;
    _app.LEVs = [];
    _app.Editors = [];
    _app.EditorFactories = {};


    _app.dateFormatStrings = {
        '{0:g}': dateTimeFormat.ShortDatePattern + ' ' + dateTimeFormat.ShortTimePattern,
        '{0:G}': dateTimeFormat.ShortDatePattern + ' ' + dateTimeFormat.LongTimePattern,
        '{0:f}': dateTimeFormat.LongDatePattern + ' ' + dateTimeFormat.ShortTimePattern,
        '{0:u}': dateTimeFormat.SortableDateTimePattern,
        '{0:U}': dateTimeFormat.UniversalSortableDateTimePattern
    }

    Sys.Application.add_load(function () {
        if (!_touch)
            _app._parse();
    });
    if (!$.mobile) {
        Sys.Application.add_load(_app._load);
        Sys.Application.add_unload(_app._unload);
    }

    _window.$createDataView = function (placeholderId, controller, args) {
        var params = { 'id': controller + 'Extender', 'controller': controller, 'baseUrl': './', 'servicePath': 'Services/DataControllerService.asmx' }
        if (args) {
            for (var i = 0; i < args.length; i++)
                params[args[i].name] = args[i].value;
        }
        $create(Web.DataView, params, null, null, $get(placeholderId));
    }

    _window.updateACT = function () {
        if (Sys.Extended && typeof (AjaxControlToolkit) == "undefined") AjaxControlToolkit = Sys.Extended.UI;
        Web.AutoComplete.registerClass('Web.AutoComplete', AjaxControlToolkit.AutoCompleteBehavior);
        Sys.Browser.WebKit = {};
        if (navigator.userAgent.indexOf('WebKit/') > -1) {
            Sys.Browser.agent = Sys.Browser.WebKit;
            Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
            Sys.Browser.name = 'WebKit';
        }
        //$common.getScrolling = function () {
        //    var x = 0;
        //    var y = 0;
        //    if (window.pageYOffset) {
        //        y = window.pageYOffset;
        //        x = window.pageXOffset;
        //    } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        //        y = document.body.scrollTop;
        //        x = document.body.scrollLeft;
        //    } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        //        y = document.documentElement.scrollTop;
        //        x = document.documentElement.scrollLeft;
        //    }
        //    return new Sys.UI.Point(x, y);
        //}
        if (!(Sys.Browser.agent == Sys.Browser.InternetExplorer || Sys.Browser.agent == Sys.Browser.Firefox || Sys.Browser.agent == Sys.Browser.Opera) && !Sys.Extended) {
            $common.old_getBounds = $common.getBounds;
            $common.getBounds = function (element) {
                var bounds = $common.old_getBounds(element);
                var scrolling = _app.scrolling(); // $common.getScrolling();
                if (scrolling.y || scrolling.x) {
                    bounds.x += scrolling.x;
                    bounds.y += scrolling.y;
                }
                return bounds;
            }
        }
        if (AjaxControlToolkit.CalendarBehavior && !AjaxControlToolkit.CalendarBehavior.prototype.old_show) {
            AjaxControlToolkit.CalendarBehavior.prototype.old_show = AjaxControlToolkit.CalendarBehavior.prototype.show;
            AjaxControlToolkit.CalendarBehavior.prototype.show = function () {
                this.old_show();
                //this._container.style.zIndex = 100100;
                var container = $(this._container).zIndex(100100),
                    elem = $(this._element),
                    showAbove = elem.parent().find('.Error:visible').length || elem.closest('div.Item').find('.Error:visible').length;
                function positionCalendar() {
                    container.position({ my: 'left ' + (showAbove ? 'bottom' : 'top'), at: 'left ' + (showAbove ? 'top' : 'bottom'), of: elem });
                }
                positionCalendar();
                setTimeout(positionCalendar, 10);
            }
            AjaxControlToolkit.CalendarBehavior.prototype.old_raiseDateSelectionChanged = AjaxControlToolkit.CalendarBehavior.prototype.raiseDateSelectionChanged;
            AjaxControlToolkit.CalendarBehavior.prototype.raiseDateSelectionChanged = function () {
                this.old_raiseDateSelectionChanged();
                this._element.focus();
            }
        }
        if (AjaxControlToolkit.TabContainer && !AjaxControlToolkit.TabContainer.prototype.old_set_activeTabIndex) {
            AjaxControlToolkit.TabContainer.prototype.old_set_activeTabIndex = AjaxControlToolkit.TabContainer.prototype.set_activeTabIndex;
            AjaxControlToolkit.TabContainer.prototype.set_activeTabIndex = function (value) {
                var oldActiveTabIndex = this.get_activeTabIndex();
                if (!this._headerAssigned) {
                    this._headerAssigned = true;
                    Sys.UI.DomElement.addCssClass(this._element.getElementsByTagName('div')[0], 'tab-header');
                }
                this.old_set_activeTabIndex(value);
                if (value != oldActiveTabIndex)
                    _body_performResize();
            }
        }
        if (AjaxControlToolkit.AutoCompleteBehavior && !AjaxControlToolkit.AutoCompleteBehavior.prototype.old_dispose) {
            AjaxControlToolkit.AutoCompleteBehavior.prototype.old_dispose = AjaxControlToolkit.AutoCompleteBehavior.prototype.dispose;
            AjaxControlToolkit.AutoCompleteBehavior.prototype.dispose = function () {
                this.old_dispose();
                if (this._completionListElement) {
                    this._completionListElement.parentNode.removeChild(this._completionListElement);
                    delete this._completionListElement;
                }
            }
            AjaxControlToolkit.AutoCompleteBehavior.prototype.old__handleFlyoutFocus = AjaxControlToolkit.AutoCompleteBehavior.prototype._handleFlyoutFocus;
            AjaxControlToolkit.AutoCompleteBehavior.prototype._handleFlyoutFocus = function () {
                if (!this._completionListElement) return;
                this.old__handleFlyoutFocus();
            }
            AjaxControlToolkit.AutoCompleteBehavior.prototype.old_showPopup = AjaxControlToolkit.AutoCompleteBehavior.prototype.showPopup;
            AjaxControlToolkit.AutoCompleteBehavior.prototype.showPopup = function () {
                this.old_showPopup();
                if (Sys.UI.DomElement.getVisible(this._completionListElement)) {
                    var scrolling = _app.scrolling(); // $common.getScrolling();
                    this._completionListElement.style.height = '';
                    Sys.UI.DomElement.addCssClass(this._completionListElement, 'CompletionList');
                    this._completionListElement.style.width = '';
                    this._completionListElement.style.zIndex = 200100;
                    var cb = $common.getClientBounds();
                    var bounds = $common.getBounds(this._completionListElement);
                    if (bounds.width > cb.width / 3) bounds.width = cb.width / 3;
                    var elem = this._element;
                    if (Sys.UI.DomElement.containsCssClass(elem.parentNode, 'Input'))
                        while (!Sys.UI.DomElement.containsCssClass(elem.parentNode, 'AutoCompleteFrame'))
                            elem = elem.parentNode;
                    var elemBounds = $common.getBounds(elem);
                    var borderBox = $common.getBorderBox(this._completionListElement);
                    var paddingBox = $common.getPaddingBox(this._completionListElement);
                    if (bounds.width <= elemBounds.width)
                        this._completionListElement.style.width = (elemBounds.width - borderBox.horizontal - paddingBox.horizontal) + 'px';
                    bounds = $common.getBounds(this._completionListElement);
                    if (bounds.x != elemBounds.x) {
                        bounds.x = elemBounds.x;
                        this._completionListElement.style.left = bounds.x + 'px';
                    }
                    if (bounds.y != elemBounds.y) {
                        bounds.y = elemBounds.y + elemBounds.height - 1;
                        this._completionListElement.style.top = bounds.y + 'px';
                    }
                    if (bounds.x + bounds.width > cb.width)
                        this._completionListElement.style.left = (cb.width - bounds.width) + 'px';
                    bound = $common.getBounds(this._completionListElement);
                    var spaceAbove = elemBounds.y - scrolling.y;
                    var spaceBelow = cb.height - (elemBounds.y + elemBounds.height - scrolling.y);
                    if (bound.height <= spaceBelow || spaceBelow >= spaceAbove) {
                        if (bounds.y + bounds.height - scrolling.y > cb.height) {
                            this._completionListElement.style.height = (cb.height - (bounds.y - scrolling.y) - 4) + 'px';
                            this._completionListElement.style.overflow = 'auto';
                        }
                    }
                    else {
                        if (spaceAbove < bounds.height) {
                            this._completionListElement.style.top = (elemBounds.y - spaceAbove) + 'px';
                            this._completionListElement.style.height = spaceAbove + 'px';
                            this._completionListElement.style.overflow = 'auto';
                        }
                        else
                            this._completionListElement.style.top = (elemBounds.y - bounds.height + 3) + 'px';
                    }
                }
            }
            if (AjaxControlToolkit.ModalPopupBehavior && !AjaxControlToolkit.ModalPopupBehavior.prototype.old__attachPopup) {
                AjaxControlToolkit.ModalPopupBehavior.prototype.old__attachPopup = AjaxControlToolkit.ModalPopupBehavior.prototype._attachPopup;
                AjaxControlToolkit.ModalPopupBehavior.prototype._attachPopup = function () {
                    this.old__attachPopup();
                    if (this._dropShadowBehavior /*&& __targetFramework != '3.5'*/) {
                        this._dropShadowBehavior.set_Width(4);
                        if (Sys.Browser.agent != Sys.Browser.InternetExplorer || Sys.Browser.version >= 9)
                            this._dropShadowBehavior.set_Rounded(true);
                    }
                }
            }
        }
    }

    _window.$hoverTab = function (elem, active) {
        while (elem && elem.tagName != 'TD')
            elem = elem.parentNode;
        if (elem) {
            if (active) {
                Sys.UI.DomElement.addCssClass(elem, 'Active');
                elem.focus();
            }
            else
                Sys.UI.DomElement.removeCssClass(elem, 'Active');
        }
    }

    _window.$getSideBar = function () {
        var sideBar = $get('PageContentSideBar');
        if (!sideBar) return null;
        for (var i = 0; i < sideBar.childNodes.length; i++) {
            var n = sideBar.childNodes[i];
            if (n.className == 'SideBarBody') return n;
        }
        return null;
    }

    _window.$dvget = function (controller, view, fieldName, containerOnly) {
        var list = Sys.Application.getComponents();
        var cid = '_' + controller;
        var dataView = null;
        for (var i = 0; i < list.length; i++) {
            var c = list[i];
            if (c._id == controller || _app.isInstanceOfType(c) && (c._id.endsWith(cid) || c._controller == controller && (!view || c.get_viewId() == view))) {
                dataView = c;
                break;
            }
        }
        if (dataView) {
            if (fieldName) {
                var field = dataView.findField(fieldName);
                if (field) {
                    if (containerOnly) {
                        element = $get(dataView._id + '_ItemContainer' + field.Index);
                        if (element)
                            for (i = 0; i < element.childNodes.length; i++) {
                                var velem = element.childNodes[i];
                                if (velem.className == 'Value')
                                    return velem;
                            }
                    } else
                        return $get(dataView._id + '_Item' + field.Index)
                    return element;
                }
                else
                    return null;
            }
            else
                return dataView;
        }
        return null;
    }

    Sys.UI.DomElement.setFocus = function (element) {
        var sel = document.selection;
        if (sel && sel.type != 'Text' && sel.type != 'None')
            sel.clear();
        if (element) {
            element.focus();
            if (element.select)
                element.select();
            if (document.focus)
                document.focus();
        }
    }

    Sys.UI.DomElement.getCaretPosition = function (element) {
        var caretPos = 0;     // IE Support     
        if (document.selection) {
            element.focus();
            var sel = document.selection.createRange();
            sel.moveStart('character', -element.value.length);
            caretPos = sel.text.length;
        }     // Firefox support     
        else if (element.selectionStart || element.selectionStart == '0')
            caretPos = element.selectionStart;
        return (caretPos);
    }

    Sys.UI.DomElement.setCaretPosition = function (element, pos) {
        if (element.setSelectionRange) {
            element.focus();
            element.setSelectionRange(pos, pos);
        }
        else if (element.createTextRange) {
            var range = element.createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    }

    _field_prepareDataFormatString = function (dataView, field) {
        if (field.DataFormatString && field.DataFormatString.indexOf('{') == -1) field.DataFormatString = '{0:' + field.DataFormatString + '}';
        if (field.DataFormatString) field.DataFormatString = dataView.resolveClientUrl(field.DataFormatString);
        if (field.Type.startsWith('Date')) {
            if (!field.DataFormatString) field.DataFormatString = '{0:d}';
            else {
                var m = field.DataFormatString.match(/{0:(g)}/i),
                    fmt;
                if (m) {
                    field.DateFmtStr = '{0:' + dateTimeFormat.ShortDatePattern + '}';
                    field.TimeFmtStr = '{0:' + (m[1] == 'g' ? dateTimeFormat.ShortTimePattern : dateTimeFormat.LongTimePattern) + '}';
                }
                fmt = _app.dateFormatStrings[field.DataFormatString];
                if (fmt) field.DataFormatString = '{0:' + fmt + '}';
                if (field.DateFmtStr)
                    field.DataFmtStr = field.DataFormatString;
            }
        }
    }

    _app._tagTests = {};
    _isTagged = _app.is = function (tags, test) {
        var re, m, result = false, v;
        if (tags) {
            re = _app._tagTests[test];
            if (!re) {
                re = new RegExp('(^|\\s|,)' + RegExp.escape(test) + '(-(\\w+))?\\b');
                _app._tagTests[test] = re;
            }
            m = tags.match(re);
            if (m) {
                v = m[3];
                if (v == null)
                    result = true;
                else if (v != 'none')
                    result = v;
            }
        }
        return result;
    }

    _field_is = function (test) {
        return _isTagged(this.Tag, test);
    }

    _field_tagged = function () {
        var tag = this.Tag,
            index,
            t, number = '',
            argList = arguments;
        if (tag) {
            //if (argList.length == 1 && Object.prototype.toString.call(argList[0]) == '[object Array]')
            //    argList = argList[0];
            for (var i = 0; i < argList.length; i++) {
                t = argList[i];
                index = tag.indexOf(t);
                if (index >= 0) {
                    i = index + t.length;
                    while (i < tag.length) {
                        var m = tag[i].match(/\d/);
                        if (m)
                            number += tag[i++];
                        else
                            break;
                    }
                    _app.tagSuffix = number;
                    return true;
                }
            }
        }
        return false;
    }

    _field_tag = function (t) {
        this.Tag = (this.Tag || '') + ',' + t;
    };

    _field_lov = function (kind) {
        var that = this,
            itemsDataController = that.ItemsDataController;
        if (kind == 'dynamic')
            return !!itemsDataController;
        if (kind == 'static')
            return !itemsDataController && !!that.ItemsStyle;
        return that.Items;
    }

    _field_format = function (v) {
        var that = this,
            formatOnClient = that.FormatOnClient,
            dataFormatString = that.DataFormatString,
            result;
        try {
            if (v != null && that.Type == 'TimeSpan' && dataFormatString && formatOnClient && typeof v == 'string')
                v = Date.tryParseFuzzyTime(v, false);

            if (v == null)
                result = 'null';
            else {
                //if (that._smartSize && !dataFormatString)
                //    result = fileSizeToText(v);
                //else
                result = formatOnClient && !isNullOrEmpty(dataFormatString) ? String.localeFormat(dataFormatString, v) : v.toString();
            }
            return result;
        }
        catch (e) { throw new Error(String.format('\nField: {0}\nData Format String: {1}\n{2}', that.Name, dataFormatString, e.message)) }
    }

    _field_isReadOnly = function () {
        return this.TextMode == 4 || this.ReadOnly;
    }

    _field_isNumber = function () {
        return Array.indexOf(['SByte', 'Byte', 'Int16', 'Int32', 'UInt32', 'Int64', 'Single', 'Double', 'Decimal', 'Currency'], this.Type)
    }

    _field_htmlEncode = function () {
        return this.HtmlEncode && this.TextMode != 2;
    }

    _field_trim = function (v) {
        var that = this;
        if (that.Type == 'String' && v != null && v.length > resourcesData.MaxReadOnlyStringLen && !(that.TextMode == 3 || that.TextMode == 2 || !that.HtmlEncode && v.match(/<\/?\w+>/)))
            v = v.substring(0, resourcesData.MaxReadOnlyStringLen) + '...';
        //if (v && that.TextMode == 3)
        //    v = v.replace(/(\r?\n)/g, '<br/>');
        return v;
    }

    _field_text = function (v, trim) {
        var that = this,
            s, valueList,
            nullValue = resourcesData.NullValueInForms,
            fieldItems = that.DynamicItems || that.Items;
        if (fieldItems.length == 0)
            if (isBlank(v))
                s = nullValue;
            else {
                if (that.Type == 'Byte[]' && !that.OnDemand)
                    s = _app.toHexString(v);
                else
                    s = that.format(v);
            }
        else if (that.ItemsDataController) {
            //var lov = v ? v.split(',') : [],
            //    sb = new Sys.StringBuilder(),
            //    firstItem = true;
            //for (var i = 0; i < fieldItems.length; i++) {
            //    var item = fieldItems[i],
            //        itemValue = item[0] == null ? '' : item[0].toString();
            //    if (Array.contains(lov, itemValue)) {
            //        if (firstItem)
            //            firstItem = false;
            //        else
            //            sb.append(', ');
            //        sb.append(item[1]);
            //    }
            //}

            //s = sb.toString();
            s = [];
            if (v && typeof v !== 'string')
                v = v.toString();
            if (that.ItemsTargetController || that.ItemsStyle == 'CheckBoxList') {
                //valueList = (v || '').split(',' _app._simpleListRegex);
                valueList = (v || '').split(',');
                $(fieldItems).each(function () {
                    var item = this;
                    if (valueList.indexOf((item[0] || '').toString()) != -1)
                        s.push(item[1]);
                });
            }
            else
                $((v || '').split(',')).each(function (index) {
                    var item = that._dataView._findItemByValue(that, this);
                    if (item)
                        s.push(item[1]);
                });
            s = s.length == 0 ? nullValue : s.join(', ');
        }
        else {
            if (v == null)
                s = nullValue;
            else {
                var item = that._dataView._findItemByValue(that, v);
                s = item[0] == v ? item[1] : v;
            }
        }
        if (that.TextMode == 1) s = '**********';
        if (trim != false)
            s = that.trim(s);
        return s;
    }

    Array.indexOfCaseInsensitive = function (list, value) {
        value = value.toLowerCase();
        for (var i = 0; i < list.length; i++)
            if (list[i].toLowerCase() == value)
                return i;
        return -1;
    }

    Number.tryParse = function (s, fmt) {
        if (typeof (s) != 'string') return null;
        if (isNullOrEmpty(s)) return null;
        var n = Number.parseLocale(s);
        if (isNaN(n)) {
            var nf = Sys.CultureInfo.CurrentCulture.numberFormat;
            if (!nf._simplifyRegex)
                nf._simplifyRegex = new RegExp(String.format('({0}|\\{1})', nf.CurrencySymbol.replace(/(\W)/g, "\\$1"), nf.CurrencyGroupSeparator), 'gi');
            var isNegative = s.match(/\(/) != null;
            s = s.replace(nf._simplifyRegex, '').replace(/\(|\)/g, '');
            s = s.replace(nf.CurrencyDecimalSeparator, nf.NumberDecimalSeparator);
            n = Number.parseLocale(s)
            if (isNaN(n)) {
                n = Number.parseLocale(s.replace(nf.PercentSymbol, ''));
                if (!isNaN(n))
                    n /= 100;
            }
        }
        if (!isNaN(n)) {
            if (isNegative)
                n *= -1;
            return n;
        }
        return null;
    }

    Date.tryParseFuzzyDate = function (s, dataFormatString) {
        if (isNullOrEmpty(s)) return null;
        s = s.trim();
        var d = Date.parseLocale(s, dateTimeFormat.ShortDatePattern);
        if (d == null)
            d = Date.parseLocale(s, dateTimeFormat.LongDatePattern);
        if (d == null && !isNullOrEmpty(dataFormatString)) {
            var dfsm = dataFormatString.match(/\{0:([\s\S]*?)\}/);
            if (dfsm)
                d = Date.parseLocale(s, dfsm[1]);
        }
        if (d)
            return d;
        // month or day name
        d = new Date();
        var m = s.match(/^(\w+)$/);
        if (m) {
            var index = Array.indexOfCaseInsensitive(dateTimeFormat.DayNames, m[1]);
            if (index == -1)
                index = Array.indexOfCaseInsensitive(dateTimeFormat.AbbreviatedDayNames, m[1]);
            if (index == -1)
                index = Array.indexOfCaseInsensitive(dateTimeFormat.ShortestDayNames, m[1]);
            if (index >= 0) {
                while (d.getDay() != index)
                    d.setDate(d.getDate() + 1);
                return d;
            }
        }
        // month and day
        m = s.match(/^(\w+|\d+)[^\w\d]*(\w+|\d+)$/);
        if (m) {
            var month = m[1];
            var day = m[2];
            if (month.match(/\d+/)) {
                month = day;
                day = m[1];
            }
            m = day.match(/\d+/);
            day = m ? m[0] : 1;
            index = Array.indexOfCaseInsensitive(dateTimeFormat.MonthNames, month);
            if (index == -1)
                index = Array.indexOfCaseInsensitive(dateTimeFormat.AbbreviatedMonthNames, month);
            if (index >= 0) {
                d.setDate(1);
                while (d.getMonth() != index)
                    d.setMonth(d.getMonth() + 1);
                d.setDate(day);
                return d;
            }
        }
        // try converting numbers
        m = s.match(/^(\d\d?)(\D*(\d\d?))?(\D*(\d\d\d?\d?))?$/);
        if (!m) return null;
        try {
            if (!dateTimeFormat.LogicalYearPosition) {
                var ami = dateTimeFormat.ShortDatePattern.indexOf('m');
                if (ami < 0)
                    ami = dateTimeFormat.ShortDatePattern.indexOf('M');
                var adi = dateTimeFormat.ShortDatePattern.indexOf('d');
                if (adi < 0)
                    adi = dateTimeFormat.ShortDatePattern.indexOf('D');
                dateTimeFormat.LogicalYearPosition = 5;
                dateTimeFormat.LogicalMonthPosition = ami < adi ? 1 : 3;
                dateTimeFormat.LogicalDayPosition = ami < adi ? 3 : 1;
            }
            var dy = m[dateTimeFormat.LogicalYearPosition];
            // find year
            if (isNullOrEmpty(dy))
                dy = d.getFullYear();
            else
                dy = Number.parseLocale(dy);
            if (!isNaN(dy) && dy < 50)
                dy += !dateTimeFormat.Calendar.convert ? 2000 : 1400;
            // find month
            var dm = m[dateTimeFormat.LogicalMonthPosition];
            if (isNullOrEmpty(dm))
                dm = d.getMonth();
            else {
                dm = Number.parseLocale(dm);
                dm--;
            }
            // find day
            var dd = m[dateTimeFormat.LogicalDayPosition];
            if (isNullOrEmpty(dd))
                dd = d.getDate();
            else
                dd = Number.parseLocale(dd);
            d = new Date(dy, dm, dd);
            if (isNaN(d.getTime()))
                return null;
        }
        catch (err) {
            return null;
        }
        return d;
    }

    Date.tryParseFuzzyTime = function (s, autoAdjustHours) {
        if (isNullOrEmpty(s)) return null;
        s = s.trim();
        var d = null;
        var m = s.match(/^(\d\d?)(\D*(\d\d?))?(\s*(\w+))?$/);
        if (!m)
            m = s.match(/^(\d\d?)(\D*(\d\d?))?(\D*(\d\d?))?(\D*(\d+))?(\s*([\S\s]+))?$/);
        if (m) {
            d = new Date();
            var hh = m[1];
            var mm = m[3] || '0';
            var ss = m.length == 10 ? m[5] : '0' || '0';
            var ms = m.length == 10 ? m[7] : '0' || '0';
            var ampm = m[m.length - 1];
            if (!isNullOrEmpty(hh)) {
                hh = Number.parseLocale(hh);
                if (!isNullOrEmpty(ampm))
                    if (ampm.toLowerCase() == dateTimeFormat.PMDesignator.toLowerCase()) {
                        if (hh != 12)
                            hh += 12
                    }
                    else {
                        if (hh == 12)
                            hh = 0
                    }
                else
                    if (autoAdjustHours != false && !isNullOrEmpty(dateTimeFormat.PMDesignator) && dateTimeFormat.ShortTimePattern.indexOf('tt') > 0 && new Date().getHours() >= 12)
                        hh += 12;
                d.setHours(hh);
            }

            if (!isNullOrEmpty(mm))
                d.setMinutes(Number.parseLocale(mm));
            d.setSeconds(!isNullOrEmpty(ss) ? Number.parseLocale(ss) : 0);
            d.setMilliseconds(!isNullOrEmpty(ms) ? Number.parseLocale(ms) : 0);
        }
        return d;
    }

    Date.tryParseFuzzyDateTime = function (s, dataFormatString) {
        s = s.trim();
        var m = s.match(/([^\s]+)\s+(.+)/);
        if (m) {
            // see if second part is am/pm
            if (!isNullOrEmpty(dateTimeFormat.AMDesignator) && (m[2].toLowerCase() == dateTimeFormat.AMDesignator.toLowerCase() || m[2].toLowerCase() == dateTimeFormat.PMDesignator.toLowerCase()))
                return Date.tryParseFuzzyTime(s);

            var date = Date.tryParseFuzzyDate(m[1], dataFormatString);
            var time = Date.tryParseFuzzyTime(m[2], true);
            if (!date && !time)
                return null;
            else if (date && !time)
                return date;
            else if (!date && time)
                return time;
            else {
                time.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
                return time;
            }
        }
        return null;
    }

    Date._jsonRegex = new RegExp(/(\d{4})\-(\d{2})\-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})\.(\d{3})(Z)?/);
    Date.fromUTCString = function (s) {
        var m = Date._jsonRegex.exec(s),
            d;
        if (m)
            d = new Date(parseInteger(m[1]), parseInteger(m[2]) - 1, parseInteger(m[3]), parseInteger(m[4]), parseInteger(m[5]), parseInteger(m[6]), parseInteger(m[7]));
        //var offset = d.getTimezoneOffset();
        //d.setMinutes(d.getMinutes() + offset);
        return d;
    };

    function deserializeControllerJson(data) {
        return _app._deserializeDates != false ? data.replace(/\"(\d{4}\-\d{2}\-\d{2}T\d{2}\:\d{2}\:\d{2}\.\d{3})Z?\"/g, 'Date.fromUTCString("$1")') : data;
    }

    _app.parseJSON = function (result) {
        return result ? eval('(' + deserializeControllerJson(result) + ')').d : null
    }

    _serializer.deserialize = function (data, secure) {
        var er, exp, result;
        try {
            //exp = data.replace(/\"(\d{4}\-\d{2}\-\d{2}T\d{2}\:\d{2}\:\d{2}\.\d{3})Z?\"/g, 'Date.fromUTCString("$1")');
            exp = deserializeControllerJson(data);
            //result = baseDeserializeMethod(exp, secure);
            result = eval('(' + exp + ')');
            return result;
        }
        catch (er) {
            throw Error.argument('data', Sys.Res.cannotDeserializeInvalidJson);
        }
    }

    Date.$addDays = function (d, delta) {
        return d ? new Date(d.setDate(d.getDate() + delta)) : d;
    }

    Date.$now = function () {
        return new Date();
    }

    Date.$today = function () {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0);
    }

    Date.$endOfDay = function () {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59);
    }

    Date.$within = function (d, delta) {
        return d ? d < Date.$addDays(Date.$today(), delta) && d >= Date.$today() : false;
    }

    Date.$pastDue = function (d1, d2) {
        if (d2 == null)
            d2 = new Date();
        if (d2.getHours() == 0)
            d2 = new Date(d2.getFullYear(), d2.getMonth(), d2.getDate(), 23, 59, 59);
        if (d1 == null) d1 = new Date();
        return d1 > d2;
    }

    Date.prototype.getFullYearText = function () {
        if (dateTimeFormat.Calendar.convert)
            return (dateTimeFormat.Calendar.convert.fromGregorian(this) || [this.getFullYear()])[0];
        return this.getFullYear();
    }

    _window.__designer = function () {
        return typeof __designerMode != 'undefined';
    }

    _window.__evalEvent = function (eventName) {
        var script = this.getAttribute('on' + eventName + '2');
        if (isNullOrEmpty(script))
            return true;
        var returnResult = true;
        script = script.replace(/(^|;)return /g, ';returnResult=');
        eval(script);
        return returnResult;
    }

    function compileBusinessRule(script) {
        var compiledScript = [],
            index = 0,
            iterator = /\$(row|master|context)\s*\.\s*(\w+)(\s*=(=|(\s*([\S\s]+?)\s*;)))?/gm,
            m,
            scope, fieldName, expression, fieldExpression;
        while (m = iterator.exec(script)) {
            scope = m[1];
            fieldName = m[2];
            expression = m[6];
            compiledScript.push(script.substr(index, m.index - index));
            if (scope == 'row')
                scope = null;
            if (expression)
                compiledScript.push(String.format('this.updateFieldValue("{0}",{1});', fieldName, compileBusinessRule(expression)));
            else {
                compiledScript.push(String.format('this.fieldValue("{0}"{1})', fieldName, scope ? (',"' + scope + '"') : ''));
                if (m[4] == '=')
                    compiledScript.push('==');
            }
            index = m.index + m[0].length;
            //m = iterator.exec(script);
        }
        if (index < script.length)
            compiledScript.push(script.substr(index));
        compiledScript = compiledScript.join('').replace(/\$(row|master|context)/g, 'this.dataView().data("$1")');
        return compiledScript;
    }

    var _businessRules = _web.BusinessRules = function (dataView) {
        this._dataView = dataView;
        this.result = new Web.BusinessRules.Result(this);
    }

    _businessRules.reset = function (controller) {
        if (typeof controller !== 'string')
            controller = controller._controller;
        var rules = controllerBusinessRules[controller];
        if (rules) {
            rules.forEach(function (ruleName) {
                delete _businessRules[ruleName];
            });
            rules.splice(0);
        }
    }

    var controllerBusinessRules = _businessRules._controllers = {};

    _businessRules.prototype = {
        canceled: function () {
            return this._canceled == true;
        },
        trigger: function () {
            return this._args.trigger;
        },
        dispose: function () {
            this.reset(null);
            this._dataView = null;
            this.result._rules = null;
        },
        reset: function (args) {
            this._actionArgs = null;
            this._args = args;
        },
        _initialize: function () {
            if (!this._actionArgs) {
                var dataView = this._dataView;
                this._actionArgs = dataView._createArguments(this._args, null);
                if (dataView.editing()) {
                    dataView._validateFieldValues(this._actionArgs.Values, /*this._args.causesValidation == null || */this._args.causesValidation, false, true);
                    this._valid = dataView.validate(this._actionArgs.Values);
                }
            }
        },
        process: function (phase) {
            var that = this,
                dataView = that._dataView,
                commandName = that._args.commandName;
            if (commandName == 'Calculate' && dataView._lookupIsActive) {
                that.preventDefault();
                return;
            }

            var wdvg = _app.Geo,
                newValues;
            if (wdvg && wdvg.acquired && !(__tf != 4) && (commandName.match(/New|Edit/) || commandName.match(/Update|Insert/) && phase == 'Before')) {
                var fields = this._dataView._fields;
                for (var i = 0; i < fields.length; i++) {
                    var f = fields[i];
                    if (!f.readOnly) {
                        if (f.tagged('modified-latitude') || f.tagged('created-latitude') && commandName.match(/New|Insert/))
                            that.updateFieldValue(f.Name, wdvg.latitude == -1 ? null : wdvg.latitude);
                        else if (f.tagged('modified-longitude') || f.tagged('created-longitude') && commandName.match(/New|Insert/))
                            that.updateFieldValue(f.Name, wdvg.longitude == -1 ? null : wdvg.longitude);
                        else if (f.tagged('modified-coords') || f.tagged('created-coords') && commandName.match(/New|Insert/))
                            that.updateFieldValue(f.Name, wdvg.latitude == -1 ? null : String.format('{0},{1}', wdvg.latitude, wdvg.longitude));
                    }
                }

                newValues = that._newValues;
                if (newValues)
                    if (_touch)
                        //dataView.extension().afterCalculate(newValues);
                        _app.input.execute({ dataView: dataView, values: newValues });
                    else
                        dataView._updateCalculatedFields({ 'Errors': [], 'Values': newValues });
                that._newValues = null;
            }

            var expressions = dataView._expressions,
                controllerName = dataView._controller;
            if (!expressions || expressions.length == 0)
                return;
            if (!that._rules) {
                that._rules = [];
                var ruleIndex = 0;
                for (i = 0; i < expressions.length; i++) {
                    var exp = expressions[i], m;
                    if (exp.Type == Web.DynamicExpressionType.ClientScript && exp.Scope == Web.DynamicExpressionScope.Rule) {
                        m = exp.Result.match(/^<id>(.+?)<\/id><command>(.+?)<\/command><argument>(.*?)<\/argument><view>(.*?)<\/view><phase>(.*?)<\/phase><js>([\s\S]+?)<\/js>$/);
                        if (m) {
                            var ruleFuncName = String.format('{0}_Rule_{1}', controllerName, m[1] ? m[1] : ruleIndex),
                                ruleName = 'Web.BusinessRules.' + ruleFuncName,
                                ruleFunc = _businessRules[ruleFuncName];// eval(String.format('typeof {0} !="undefined"?{0}:null', ruleName));
                            if (!ruleFunc)
                                try {
                                    ruleFunc = eval(that._parseRule(ruleName, m[6]));
                                    if (!controllerBusinessRules[controllerName])
                                        controllerBusinessRules[controllerName] = [];
                                    controllerBusinessRules[controllerName].push(ruleFuncName);
                                }
                                catch (error) {
                                    _app.alert(String.format('{0}\n\ncommand: "{1}"\nargument: "{1}"\nview: "{3}"\nphase: "{4}"\n\n{5}', error.message, m[2], m[3] || 'n/a', m[4] || 'n/a', m[5], (m[6] || '').trim()));
                                }
                            Array.add(that._rules, { 'commandName': m[2], 'commandArgument': m[3], 'view': m[4], 'phase': m[5], 'script': ruleFunc });
                        }
                    }
                }
            }
            for (i = 0; i < that._rules.length; i++) {
                var r = that._rules[i];
                if (r.phase != phase)
                    continue;
                var skip = false;
                if (!isNullOrEmpty(r.view)) {
                    var viewId = dataView.get_viewId();
                    if (!(r.view == viewId || viewId.match(new RegExp(r.view))))
                        skip = true;
                }
                if (!isNullOrEmpty(r.commandName)) {
                    commandName = that._args.commandName;
                    if (!(r.commandName == commandName || commandName.match(new RegExp(r.commandName))))
                        skip = true;
                }
                if (!isNullOrEmpty(r.commandArgument)) {
                    var commandArgument = that._args.commandArgument;
                    if (!(r.commandArgument == commandArgument || commandArgument && commandArgument.match(new RegExp(r.commandArgument))))
                        skip = true;
                }
                if (!skip) {
                    if (that._args.causesValidation) {
                        that._initialize();
                        if (!that._valid) {
                            that.preventDefault();
                            break;
                        }
                    }
                    //if (r.commandName != 'New' || this._dataView.get_isForm())
                    r.script.call(that);
                    if (that.canceled())
                        break;
                }
            }
            if (that._newValues) {
                if (_touch)
                    //if (dataView.get_view().Layout)
                    _app.input.execute({ dataView: dataView, values: that._newValues, raiseCalculate: commandName != 'Calculate' });
                //else
                //    dataView.extension().afterCalculate(that._newValues);
                else
                    dataView._updateCalculatedFields({ 'Errors': [], 'Values': that._newValues });
                that._newValues = null;
                that._pendingFocus();
            }
            else if (that.canceled() && dataView.get_isDataSheet()) {
                dataView._updateCalculatedFields({ 'Errors': [], 'Values': [] });
                that._pendingFocus();
            }
        },
        _pendingFocus: function () {
            if (this._focusFieldName) {
                this._dataView._focus(this._focusFieldName, this._focusMessage);
                this._focusFieldName = null;
            }
        },
        before: function (args) {
            this.reset(args);
            this.process('Before');
            if (!this.canceled())
                this.process('Execute');
        },
        after: function (args) {
            if (this._dataView._isBusy)
                return;
            this.reset(args);
            this.process('After');
        },
        _parseRule: function (ruleName, script) {
            script = compileBusinessRule(script);
            var extendedScript = new Sys.StringBuilder();
            extendedScript.appendFormat('{0}=function(){{\n', ruleName);
            var iterator = /([\s\S]*?)(\[(\w+)(\.(\w+))?\](\s*=([^=][\s\S]+?);)?)/g;
            this._parseScript(extendedScript, iterator, script);
            script = extendedScript.toString();
            extendedScript.clear();
            this._parseScript(extendedScript, iterator, script);
            extendedScript.append('\n}');
            return extendedScript.toString();
        },
        _parseScript: function (sb, iterator, script) {
            var lastIndex = -1,
                m, fieldName, scope, field;
            while (m = iterator.exec(script)) {
                sb.append(m[1]);
                fieldName = m[3];
                scope = m[5];
                if (m[1].match(/\w$/))
                    sb.append(m[2]);
                else if (fieldName.match(/^master$/i)) {
                    sb.appendFormat('this.fieldValue(\'{0}\',\'Master\')', scope);
                }
                else {
                    field = this._dataView.findField(fieldName);
                    if (!isNullOrEmpty(m[7]))
                        sb.appendFormat('this.updateFieldValue(\'{0}\',{1})', fieldName, m[7]);
                    else
                        sb.appendFormat('this.fieldValue(\'{0}\',\'{1}\')', fieldName, scope);
                    //                if (field)
                    //                    if (!isNullOrEmpty(m[7]))
                    //                        sb.appendFormat('this.updateFieldValue(\'{0}\',{1});', fieldName, m[7]);
                    //                    else
                    //                        sb.appendFormat('this.fieldValue(\'{0}\',\'{1}\')', fieldName, scope);
                    //                else
                    //                    sb.append(m[2]);
                }
                lastIndex = iterator.lastIndex;
                //m = iterator.exec(script);
            }
            if (lastIndex != -1)
                sb.append(lastIndex < script.length ? script.substr(lastIndex) : '');
            else
                sb.append(script);

        },
        preventDefault: function () {
            this._canceled = true;
            this._dataView._raiseSelectedDelayed = false;
            this._dataView._pendingSelectedEvent = false;

        },
        validateInput: function () {
            this._initialize();
            return this._valid == true;
        },
        dataView: function () {
            return this._dataView;
        },
        odp: function (method) {
            var odp = this._dataView.odp,
                result = odp;
            if (method) {
                result = false;
                if (odp)
                    result = odp.is(method);
            }
            return result;
        },
        busy: function () {
            return this._dataView._busy();
        },
        arguments: function () {
            this._initialize();
            return this._actionArgs;
        },
        selectFieldValueObject: function (fieldName) {
            this._initialize();
            var values = this._actionArgs.Values,
                newValue, oldValue;
            for (var i = 0; i < values.length; i++) {
                var v = values[i];
                if (fieldName == v.Name) {
                    //newValue = v.NewValue;
                    //oldValue = v.OldValue;
                    //if (newValue != null && newValue.getTimezoneOffset)
                    //    v.NewValue = new Date(newValue + newValue.getTimezoneOffset() * 60 * 1000);
                    //if (oldValue != null && oldValue.getTimezoneOffset)
                    //    v.OldValue = new Date(oldValue + oldValue.getTimezoneOffset() * 60 * 1000);

                    return v;
                }
            }
            if (this._dataView.findField(fieldName))
                return { Name: fieldName, Value: null };
            return null;
        },
        selectFieldValue: function (fieldName) {
            var v = this.selectFieldValueObject(fieldName);
            if (v) {
                if (v.Modified)
                    return v.NewValue;
                return v.OldValue;
            }
            return null;
        },
        fieldValue: function (fieldName, type) {
            if (type == 'Master' || type == 'master') {
                var masterDataView = this._dataView.get_master();
                return masterDataView ? masterDataView.fieldValue(fieldName) : null;
            }
            if (type == 'context') {
                var parentDataView = this._dataView.get_parentDataView();
                return parentDataView ? parentDataView.fieldValue(fieldName) : null;
            }
            if (!type || type == 'Value')
                return this.selectFieldValue(fieldName);
            var v = this.selectFieldValueObject(fieldName);
            if (!v)
                this._unknownField(fieldName);
            switch (type) {
                case 'NewValue':
                    return v.NewValue;
                case 'OldValue':
                    return v.OldValue;
                case 'Modified':
                    return v.Modified;
                case 'ReadOnly':
                    return v.ReadOnly;
                case 'Error':
                    return v.Error;
                default:
                    return v.OldValue;
            }
        },
        updateFieldValue: function (fieldName, value) {
            var v = this.selectFieldValueObject(fieldName);
            if (v) {
                v.NewValue = value;
                v.Modified = true;
                var newValues = this._newValues;
                if (!newValues)
                    newValues = this._newValues = [];
                for (var i = 0; i < newValues.length; i++)
                    if (newValues[i].Name == fieldName) {
                        Array.removeAt(newValues, i);
                        break;
                    }
                Array.add(newValues, v);
            }
            else
                this._unknownField(fieldName);
        },
        property: function (name, value) {
            name = 'Rules$' + name;
            if (arguments.length == 1)
                return this._dataView.readContext(name);
            else
                this._dataView.writeContext(name, value);
        },
        _unknownField: function (fieldName) {
            throw new Error(String.format('Unknown field "{0}" is not defined in  /controllers/{1}/views/{2}.', fieldName, this._dataView._controller, this._dataView._viewId));
        }
    }

    _businessRules.Result = function (rules) {
        this._rules = rules;
    }

    _businessRules.Result.prototype = {
        focus: function (fieldName, fmt, args) {
            var message = null;
            if (arguments.length > 1) {
                var newArguments = Array.clone(arguments);
                Array.removeAt(newArguments, 0);
                message = String._toFormattedString(true, newArguments);
            }
            var rules = this._rules;
            var dataView = rules._dataView;
            if (!_touch) {
                rules._focusFieldName = fieldName;
                rules._focusMessage = message;
            }
            setTimeout(function () {
                dataView._focus(fieldName, message);
            });
        },
        showMessage: function (fmt, args) {
            _app.showMessage(String._toFormattedString(true, arguments));
        },
        showViewMessage: function (fmt, args) {
            this._rules._dataView.showViewMessage(String._toFormattedString(true, arguments));
        },
        showAlert: function (fmt, args) {
            alert(String._toFormattedString(true, arguments));
        },
        confirm: function (fmt, args) {
            return confirm(String._toFormattedString(true, arguments));
        },
        refresh: function (fetch) {
            this._rules._dataView(fetch == true);
        },
        refreshChildren: function () {
            //var dataView = this._rules._dataView;
            //dataView._forceChanged = true;
            //dataView._raiseSelectedDelayed = true;
            //this.refresh(true);
            this._rules._dataView.refreshChildren();
        }
    }

    // uploading of binary data

    var uploadSupport = {
        fileReader: typeof FileReader != 'undefined',
        formData: !!window.FormData,
        progress: 'upload' in new XMLHttpRequest,
        dragAndDrop: 'draggable' in document.createElement('span')
    };

    function filesSelected(dataView, fieldName, files) {
        var pendingUploads = dataView._pendingUploads,
            found,
            file, field,
            fileResult = { Values: [], Errors: [] };
        if (!pendingUploads)
            pendingUploads = dataView._pendingUploads = [];
        $(pendingUploads).each(function (index) {
            var upload = this;
            if (upload.fieldName == fieldName) {
                found = true;
                if (files)
                    upload.files = files;
                else {
                    upload.files = null;
                    pendingUploads.splice(index, 1);
                }
                return false;
            }
        });
        if (!found && files)
            pendingUploads.push({ fieldName: fieldName, files: files });
        if (files) {
            file = files[0];
            if (file) {

                function findField(name, prefixWithFieldName) {
                    return dataView.findField((prefixWithFieldName && fieldName || '') + name);
                }
                // file name
                field = findField('FileName', true) || findField('FILENAME', true) || findField('FILE_NAME', true) || findField('filename', true) || findField('file_name', true)
                    || findField('FileName') || findField('FILENAME') || findField('FILE_NAME') || findField('filename') || findField('file_name');
                if (field)
                    fileResult.Values.push({ Name: field.Name, NewValue: file.name });
                // content type
                field = findField('ContentType', true) || findField('CONTENTTYPE', true) || findField('CONTENT_TYPE', true) || findField('contenttype', true) || findField('conent_type', true)
                    || findField('ContentType') || findField('CONTENTTYPE') || findField('CONTENT_TYPE') || findField('contenttype') || findField('content_type');
                if (field)
                    fileResult.Values.push({ Name: field.Name, NewValue: file.type });
                // content length
                field = findField('Length', true) || findField('LENGTH', true) || findField('length', true)
                    || findField('Length') || findField('LENGTH') || findField('length');
                if (field)
                    fileResult.Values.push({ Name: field.Name, NewValue: file.size });
                if (fileResult.Values.length)
                    if (_touch)
                        //dataView.extension().afterCalculate(fileResult.Values);
                        _app.input.execute({ dataView: dataView, values: fileResult.Values });
                    else
                        dataView._updateCalculatedFields(fileResult);
            }
        }
    }

    _app.dataUrlToBlob = function (url) {
        var byteString,
            mimeString,
            byteArray, i;
        if (url.split(',')[0].indexOf('base64') >= 0)
            byteString = atob(url.split(',')[1]);
        else
            byteString = unescape(url.split(',')[1]);
        mimeString = url.split(',')[0].split(':')[1].split(';')[0];
        byteArray = new Uint8Array(byteString.length);
        for (i = 0; i < byteString.length; i++)
            byteArray[i] = byteString.charCodeAt(i);
        return new Blob([byteArray], { type: mimeString });
    }

    function findBlobContainer(dataView, blobField) {
        return $(_touch ? _touch.page(dataView._id) : dataView._container).find('.drop-box-' + blobField.Index);
    }

    function sizeToText(size) {
        if (size == null)
            return '';
        var format,
            suffix;
        if (size > 1024) {
            if (size > 1000000) {
                if (size > 10000000)
                    if (size > 100000000)
                        format = '{0:N0}';
                    else
                        format = '{0:N1}';
                else
                    format = '{0:N2}';
                size /= 1048576
                suffix = resourcesFiles.MB;
            }
            else {
                if (size > 10240)
                    if (size > 102400)
                        format = '{0:N0}';
                    else
                        format = '{0:N1}';
                else
                    format = '{0:N2}';
                size /= 1024;
                suffix = resourcesFiles.KB;
            }
        }
        else {
            format = '{0}';
            suffix = resourcesFiles.Bytes
        }
        return String.format(format, size) + ' ' + suffix;
    }

    _app.upload = function (method, options) {
        if (!arguments.length || !uploadSupport.formData)
            return uploadSupport.formData;

        var container = $(options.container),
            clickEvent = _touch ? 'vclick' : 'click';

        switch (method) {
            case 'create':
                initialize();
                break;
            case 'destroy':
                destroy();
                break;
            case 'execute':
                uploadFiles();
                break;
            case 'resize':
                var screenWidth = container.data('screen-width');
                if (screenWidth == null || screenWidth != $window.width()) {
                    if (container.is(':visible'))
                        container.data('screen-width', $window.width());
                    resizeSignature();
                }
                break;
            case 'validate':
                return validate();
        }

        function resizeSignature() {
            var ratio = _window.devicePixelRatio || 1,
                canvas = container.find('canvas'),
                w, ctx;
            if (canvas.length) {
                w = canvas.width();
                canvas.height(w * .5);
                canvas = canvas.get(0);
                canvas.width = canvas.offsetWidth * ratio;
                canvas.height = canvas.offsetHeight * ratio;
                ctx = canvas.getContext("2d");
                ctx.scale(ratio, ratio);
                container.data('signature').clear();
                ctx.fillStyle = '#ccc';
                ctx.font = "20pt Arial";
                ctx.textAlign = 'center';
                ctx.fillText(resourcesMobile.Files.Sign, w / 2, w * .25);
            }
        }

        function validate() {
            var dv = findDataView(options.dataViewId),
                blobField = dv.findField(options.fieldName),
                signature = container.data('signature'),
                row = _touch ? dv.extension().commandRow() : dv.get_currentRow(),
                v = row && row[blobField.Index],
                result = v != null && !v.toString().match(/^null\|/),
                signed;
            if (container.is('.app-signature')) {
                signed = !signature.isEmpty();
                result = result || signed;
                filesSelected(dv, options.fieldName, signed ? _app.dataUrlToBlob(signature.toDataURL('image/png', 1)) : null);
            }
            else
                result = result || container.find('div').length > 0;
            return result;
        }

        function configureEmpty() {
            var multiple = options.multiple;
            container.empty().addClass('app-drop-box app-empty');
            if (uploadSupport.dragAndDrop && (!_touch || _touch.desktop()))
                container.text(multiple ? resourcesFiles.DropMany : resourcesFiles.Drop);
            else
                container.text(_touch ? (multiple ? resourcesFiles.TapMany : resourcesFiles.Tap) : (multiple ? resourcesFiles.ClickMany : resourcesFiles.Click));
            container.attr('title', _touch ? (multiple ? resourcesFiles.TapMany : resourcesFiles.Tap) : (multiple ? resourcesFiles.ClickMany : resourcesFiles.Click));
        }

        function initialize() {
            if (!container.is('.app-drop-box')) {
                configureEmpty();

                var dataView = findDataView(options.dataViewId),
                    field = dataView.findField(options.fieldName);

                if (field.OnDemandStyle == 2) {
                    container.empty().attr('title', resourcesFiles.Sign);
                    var canvas = $('<canvas style="width:100%"></canvas>').appendTo(container).get(0);
                    container.addClass('app-signature').data('signature', new SignaturePad(canvas, { backgroundColor: 'white' }));
                    // ui-btn-icon-notext ui-icon-delete 
                    $('<a class="app-clear ui-btn ui-btn-inline ui-corner-all"/>').insertAfter(container).text(resourcesFiles.Clear).attr('title', resourcesFiles.Clear)
                        .on(clickEvent, function (e) {
                            if (container.data('signature').isEmpty()) {
                                if (_touch)
                                    _touch.activeLink();
                                return;
                            }
                            container.addClass('app-dragging');
                            _app.confirm(resourcesFiles.ClearConfirm, function () {
                                container.removeClass('app-dragging').focus();
                                resizeSignature();
                                if (options.dataViewId) {
                                    var dataView = findDataView(options.dataViewId);
                                    filesSelected(dataView, options.fieldName, null);
                                }
                                if (options.change)
                                    options.change();
                            },
                                function () {
                                    container.removeClass('app-dragging').focus();
                                });
                        });
                    if (!_touch)
                        resizeSignature();
                }
                else {
                    var input = $('<input type="file"/>').insertAfter(container).hide().on('change', function (e) {
                        var files = this.files;
                        if (files.length > 0)
                            captureFiles(files);
                    });
                    if (options.multiple)
                        input.attr('multiple', '');
                    container.on('dragenter', function () {
                        $(this).addClass('app-dragging');
                        return false;
                    }).on('dragover', function () {
                        $(this).addClass('app-dragging');
                        return false;
                    }).on('dragend dragleave', function (e) {
                        $(this).removeClass('app-dragging');
                        return false;
                    }).on('drop', function (e) {
                        $(this).removeClass('app-dragging');
                        e.preventDefault();
                        if (!container.is('.app-uploading'))
                            captureFiles(e.originalEvent.dataTransfer.files);
                    }).on(clickEvent, function (e) {
                        if (!container.is('.app-uploading'))
                            if ($(e.target).closest('.app-clear').length) {
                                container.addClass('app-dragging')
                                _app.confirm(container.find('div').map(function () { return $(this).text() }).get().join(', ') + '\n\n' + resourcesFiles.ClearConfirm, function () {
                                    container.removeClass('app-dragging').focus();
                                    configureEmpty();
                                    container.next().val('');
                                    if (options.dataViewId) {
                                        var dataView = findDataView(options.dataViewId);
                                        filesSelected(dataView, options.fieldName, null);
                                    }
                                    if (options.change)
                                        options.change();
                                    return false;
                                },
                                    function () {
                                        container.removeClass('app-dragging').focus();
                                    });
                            }
                            else
                                try {
                                    container.next()[0].click();//.trigger('click');
                                }
                                catch (ex) {
                                    // W10 throws "Operation aborted" exception in WebView when user cancels file selector.
                                    // using .trigger('click') will make jquery 2.2.4 inoperable
                                }
                        return false;
                    });
                }
            }
        }

        function destroy() {
            options.change = null;
            if (container.is('.app-signature')) {
                container.removeData().find('canvas').off();
                container.next().off().remove();
            }
            else
                container.off().addClass('app-drop-box-destroyed').next().off().remove();
        }

        function captureFiles(files) {
            if (!options.multiple && files.length > 1)
                files = [files[0]];

            var imageFiles = 0,
                dv = findDataView(options.dataViewId),
                modalHeight = dv && !_touch && dv.get_isModal() ? $(dv._container).height() : 0;
            if (!container.is(':empty'))
                container.css('min-height', container.height());
            container.find('img div').off();
            container.empty().removeClass('app-empty');
            $(files).each(function (index) {
                var file = this,
                    fileNumber = files.length > 1 ? (index + 1).toString() + '. ' : '',
                    size = sizeToText(file.size),
                    fileInfo = $('<div></div>').appendTo(container).text(fileNumber + file.name + ' - ' + size);
                if (uploadSupport.fileReader && file.type.match(/^image\//i)) {
                    imageFiles++;
                    var reader = new FileReader();
                    reader._fileInfo = fileInfo;
                    reader.onload = function (event) {
                        $('<img/>').attr('src', event.target.result).insertAfter(this._fileInfo).one('load', function () {
                            if (options.change)
                                options.change();
                        });
                        this._fileInfo = null;
                        if (--imageFiles == 0) {
                            container.css('min-height', '');
                            if (modalHeight)
                                $(dv._container).height(modalHeight);
                        }
                    }
                    reader.readAsDataURL(file);
                }
                if (!imageFiles) {
                    container.css('min-height', '');
                    if (options.change)
                        options.change();
                }
            });
            $('<a class="app-clear ui-btn ui-btn-icon-notext ui-icon-trash  ui-corner-all"/>').appendTo(container).text(resourcesFiles.Clear).attr('title', resourcesFiles.Clear);
            if (dv)
                filesSelected(dv, options.fieldName, files);
        }

        function uploadFiles() {
            var progress = $('<progress max="100"></progress>');
            options.container.addClass('app-uploading');
            progress.val(0).insertBefore(options.container.contents().first());
            _app.uploadFileAjax({
                url: options.url,
                files: options.files,
                progress: function (e) {
                    if (e.lengthComputable) {
                        var complete = e.loaded / e.total * 100 | 0;
                        progress.val(complete);
                    }
                }
            }).then(function (result) {
                options.container.removeClass('app-uploading');
                if (options.success)
                    options.success();
            }, function () {
                options.container.removeClass('app-uploading');
                progress.remove();
                if (options.error)
                    options.error();
            });
        }
    }

    _app.uploadFileAjax = function (options) {
        var formData = new FormData();
        $(options.files).each(function () {
            formData.append('file', this);
        });
        return $.ajax({
            url: options.url,
            method: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                if (options.progress)
                    xhr.upload.addEventListener("progress", options.progress, false);
                return xhr;
            }
        })
    };

    _window.Web$DataView$RichText = function () { }
    _window.Web$DataView$RichText.prototype = {
        attach: function (element, viewType) {
            var $element = $(element);
            $element.addClass('dataview-rich-text');
            var lang = $('html').attr('lang');
            if (isNullOrEmpty(lang))
                lang = $('html').attr('xml:lang');
            if (typeof CKEDITOR != 'undefined') {
                $('<div></div>').insertAfter($element).width($element.outerWidth()).height(1);
                CKEDITOR.replace(element.id, {
                    language: lang,
                    on:
                        {
                            instanceReady: function (ev) {
                                var originalMaximize = CKEDITOR.instances[element.id].getCommand('maximize');
                                if (originalMaximize) {
                                    originalMaximize._old_exec = originalMaximize.exec;
                                    originalMaximize.exec = function (editor) {
                                        this._old_exec(editor);
                                        $(ev.editor.container.$).find('.cke_maximized').css('z-index', 10001);
                                    }
                                }
                            }
                        }
                });
            }
            else {
                var editorResources = resources.Editor;
                var buttons = [
                    { CommandName: 'Undo', Tooltip: editorResources.Undo },
                    { CommandName: 'Redo', Tooltip: editorResources.Redo },
                    { CommandName: 'Bold', Tooltip: editorResources.Bold, ElementWhiteList: { b: ['style'], strong: ['style'] } },
                    { CommandName: 'Italic', Tooltip: editorResources.Italic, ElementWhiteList: { i: ['style'], em: ['style'] } },
                    { CommandName: 'Underline', Tooltip: editorResources.Underline, ElementWhiteList: { u: ['style'] } },
                    { CommandName: 'StrikeThrough', Tooltip: editorResources.Strikethrough, ElementWhiteList: { strike: ['style'] } },
                    { CommandName: 'Subscript', Tooltip: editorResources.Subscript, ElementWhiteList: { sub: ['style'] } },
                    { CommandName: 'Superscript', Tooltip: editorResources.Superscript, ElementWhiteList: { sup: ['style'] } },

                    { CommandName: 'JustifyLeft', Tooltip: editorResources.JustifyLeft, ElementWhiteList: { p: ['align'], div: ['style', 'align'] }, AttributeWhiteList: { style: ['text-align'] }, align: ['left'] },
                    { CommandName: 'JustifyCenter', Tooltip: editorResources.JustifyCenter, ElementWhiteList: { p: ['align'], div: ['style', 'align'] }, AttributeWhiteList: { style: ['text-align'] }, align: ['center'] },
                    { CommandName: 'JustifyRight', Tooltip: editorResources.JustifyRight, ElementWhiteList: { p: ['align'], div: ['style', 'align'] }, AttributeWhiteList: { style: ['text-align'] }, align: ['right'] },
                    { CommandName: 'JustifyFull', Tooltip: editorResources.JustifyFull, ElementWhiteList: { p: ['align'], div: ['style', 'align'] }, AttributeWhiteList: { style: ['text-align'] }, align: ['justify'] },

                    { CommandName: 'insertOrderedList', Tooltip: editorResources.InsertOrderedList, ElementWhiteList: { ol: [], li: [] } },
                    { CommandName: 'insertUnorderedList', Tooltip: editorResources.InsertUnorderedList, ElementWhiteList: { ul: [], li: [] } },

                    { CommandName: 'createLink', Tooltip: editorResources.CreateLink, ElementWhiteList: { a: ['href'] } },
                    { CommandName: 'UnLink', Tooltip: editorResources.UnLink },

                    { CommandName: 'RemoveFormat', Tooltip: editorResources.RemoveFormat },
                    { CommandName: 'SelectAll', Tooltip: editorResources.SelectAll },
                    { CommandName: 'UnSelect', Tooltip: editorResources.UnSelect },

                    { CommandName: 'Delete', Tooltip: editorResources.Delete },
                    { CommandName: 'Cut', Tooltip: editorResources.Cut },
                    { CommandName: 'Paste', Tooltip: editorResources.Paste },


                    { CommandName: 'BackColor', Tooltip: editorResources.BackColor, ElementWhiteList: { font: 'style', span: 'style' }, AttributeWhiteList: { style: 'background-color' } },
                    { CommandName: 'ForeColor', Tooltip: editorResources.ForeColor, ElementWhiteList: { font: 'color' }, AttributeWhiteList: { font: 'color' } },

                    { CommandName: 'FontName', Tooltip: editorResources.FontName, ElementWhiteList: { font: ['face'] }, AttributeWhitList: { face: [] } },
                    { CommandName: 'FontSize', Tooltip: editorResources.FontSize, ElementWhiteList: { font: ['size'] }, AttributeWhitList: { size: [] } },

                    { CommandName: 'Indent', Tooltip: editorResources.Indent, ElementWhiteList: { blockquote: ['style', 'dir'] }, AttributeWhitList: { style: ['margin', 'margin-right', 'padding', 'border'], dir: ['ltr', 'rtl', 'auto'] } },
                    { CommandName: 'Outdent', Tooltip: editorResources.Outdent },
                    { CommandName: 'InsertHorizontalRule', Tooltip: editorResources.InsertHorizontalRule, ElementWhiteList: { hr: ['size', 'width'] }, AttributeWhiteList: { size: [], width: [] } },

                    { CommandName: 'HorizontalSeparator', Tooltip: '' }
                ];
                if (navigator.appVersion.match(/MSIE/))
                    buttons.splice(0, 2);
                var type = Sys.Extended.UI.HtmlEditorExtenderBehavior;
                if (!type._customized) {
                    type.prototype.old_executeCommand = type.prototype._executeCommand;
                    type.prototype._executeCommand = function (command) {
                        $(this._editableDiv).focus();
                        this.old_executeCommand(command);
                    };
                    type._customized = true;
                }
                var editor = $create(type, {
                    ToolbarButtons: buttons, id: element.id + '$Editor'
                }, null, null, element);
                editor._noEncoding = true;
                editor._editableDiv.tabIndex = element.tabIndex;
                var topButtonContainer = $(editor._topButtonContainer);
                topButtonContainer.find('nobr span').hide();
                topButtonContainer.find('select').css({ 'font-family': '', 'font-size': '' });
                //var $editor = $(editor._editableDiv)
                //var cell = $editor.parents('.FieldWrapper').parent();
                //var cellPos = cell.offset();
                //var cellWidth = cell.width();
                //var myPos = $editor.offset();
                //var myWidth = $editor.width();
                //if (cellPos.left + cellWidth > myPos.left + myWidth) {
                //    myWidth = cellPos.left + cellWidth - myPos.left;
                //    $editor.width(myWidth);
                //}
            }
            return true;
        },
        detach: function (element, viewType) {
            if (typeof CKEDITOR != 'undefined') {
                CKEDITOR.remove(element.id);
            } else {
                var editor = $find(element.id + '$Editor');
                if (editor)
                    editor.dispose();
            }
            return true;
        },
        persist: function (element) {
            if (typeof CKEDITOR != 'undefined') {
                element.value = CKEDITOR.instances[element.id].getData();
            }
            else {
                var editor = $find(element.id + '$Editor');
                if (editor)
                    element.value = editor._editableDiv.innerHTML;
            }
        }
    }

    function prettyText(s) {
        if (s.match(/[a-z]/) && s.match(/[A-Z]/))
            s = s.replace(/([A-Z]+|\d+)/g, ' $1');
        if (s.match(/\_/))
            s = s.replace(/\_/g, ' ');
        return s.trim();
    }

    _app.prettyText = prettyText;

    _app.surveyLibrary = {};

    _app.surveyFailedToLoad = {};

    _app.action = function (options) {
        if (_touch)
            _touch.executeInContext(options.icon, options.text, options.path);
    }

    /*
    * Survey API
    */

    // .replace(/([A-Z])/g, '-(1)')

    _app.toTags = function (value, prefix) {
        if (!prefix)
            prefix = '';
        if (value != null) {
            if (typeof value == 'boolean')
                return value ? prefix : prefix + '-none';
            if (prefix)
                prefix = prefix + '-';
            if (typeof value == 'string' || typeof value == 'number')
                return prefix + value;
            var tagList = [];
            for (var k in value) {
                var result = _app.toTags(value[k], prefix + k);
                if (result)
                    tagList.push(result);
            }
            return tagList.join(' ').replace(/([A-Z])/g, '-$1').toLowerCase();
        }
        return prefix;
    }

    _app.read = function (obj, selector) {
        var path = selector.split(/\./g),
            count = path.length, name,
            current = obj, i;
        for (i = 0; i < count; i++) {
            name = path[i];
            if (i == count - 1)
                return current[name];
            else if (current[name] != null)
                current = current[name];
            else
                return null;
        }
        return null;
    }

    _app.survey = function (method, options) {
        if (typeof method !== 'string') {
            options = arguments[0];
            method = 'show';
        }
        var originalControllerName = options.controller || 'survey',
            controller = originalControllerName.replace(/\W/g, '_'),
            tags,
            values = options.values;
        if (method == 'show' && values) {
            if (!options.context)
                options.context = {};
            options.context._initVals = values;
        }
        options.external = !(options.topics || options.questions);
        //if (options.options)
        //    optionsToTags(options);

        function optionsToTags(def) {
            tags = def.tags || def.options && _app.toTags(def.options) || '';
            if (!def.text)
                tags += ' page-header-none '
            def.tags = tags;
        }

        function toUrl(name) {
            return _app.find(options.parent).get_baseUrl() + (name.match(/\//) ? name : ('/js/surveys/') + name);
        }

        function show(result) {
            try {
                //eval('$app.surveyDef=' + result);
                //var survey = $app.surveyDef;
                //$app.surveyDef = null;
                survey = eval(result);

                var layoutUrl = survey.layout || '';

                if (layoutUrl.match(/(#ref|\.html)$/i)) {
                    busy(true);
                    // load the survey from the server
                    $.ajax({
                        url: toUrl(layoutUrl == '#ref' ? (originalControllerName + '.html') : layoutUrl),
                        dataType: 'text',
                        cache: false
                    }).done(function (result) {
                        busy(false);
                        survey.layout = result;
                        doShow();
                    }).fail(function (result) {
                        busy(false);
                        if (typeof options.create == 'function')
                            options.create();
                        else
                            _app.alert('Unable to load survey layout for ' + controller + ' from the server.');
                    });
                }
                else
                    doShow();

                function doShow() {
                    showCompiled(survey);
                    if (survey.cache === false)
                        _app.surveyLibrary[controller] = null;
                }
            }
            catch (ex) {
                _app.alert('The definiton of ' + controller + ' survey is invalid.\n\n' + ex.message + (_window.location.host.match(/\localhost\b/) ? ('\n\n' + ex.stack.replace(/\n/g, '<br/>')) : ''));
            }
        }

        function ensureTopics(survey) {
            if (!survey.topics && survey.questions) {
                survey.topics = [{ questions: survey.questions }];
                survey.questions = null;
            }
        }

        function showCompiled(survey) {
            ensureTopics(survey);
            var parentId = options.parent,
                dataView = findDataView(parentId);
            survey.controller = controller;
            survey.baseUrl = dataView ? dataView.get_baseUrl() : __baseUrl;
            survey.servicePath = dataView ? dataView.get_servicePath() : __servicePath;
            survey.confirmContext = options.confirmContext;
            survey.showSearchBar = true;//dataView.get_showSearchBar();
            survey.parent = parentId;
            survey.context = options.context;
            if (options.options) {
                if (!survey.options)
                    survey.options = {};
                for (var key in options.options)
                    if (survey.options[key] == null)
                        survey.options[key] = options.options[key];
            }
            optionsToTags(survey);

            if (!survey.submit)
                survey.submit = options.submit;
            if (!survey.submitText)
                survey.submitText = options.submitText;
            if (!survey.cancel)
                survey.cancel = options.cancel;
            if (!survey.init)
                survey.init = options.init;
            if (!survey.calculate)
                survey.calculate = options.calculate;
            _app.showModal(dataView, controller, 'form1', 'New', '', survey.baseUrl, survey.servicePath, [],
                { confirmContext: options.confirmContext, showSearchBar: survey.showSearchBar, survey: survey, tags: survey.tags });
        }

        function createRule(list, funcName, func, commandName, commandArgument, phase, argument) {
            var s = 'function(){var r=this,dv=r.dataView(),s=dv.survey(),e=$.Event("' + (typeof func === 'string' ? func : funcName) +
                '",{rules:r,dataView:dv,survey:s' + (argument != null ? (',argument:' + JSON.stringify(argument)) : '') + '});' +
                (typeof func === 'string' ? '$(document).trigger(e);' : ('s.' + funcName + '(e);')) +
                (commandName == 'Calculate' ? '' : 'if(e.isDefaultPrevented())') + 'r.preventDefault();}',
                m = s.match(/^function\s*\(\)\s*\{([\s\S]+?)\}\s*$/);
            if (!commandArgument)
                commandArgument = '';
            list.push({
                "Scope": 6, "Target": null, "Type": 1, "Test": null,
                "Result": "\u003cid\u003er" + list.length + "\u003c/id\u003e\u003ccommand\u003e" + commandName + "\u003c/command\u003e\u003cargument\u003e" + commandArgument + "\u003c/argument\u003e\u003cview\u003eform1\u003c/view\u003e\u003cphase\u003e" + phase + "\u003c/phase\u003e\u003cjs\u003e" + m[1] + "\u003c/js\u003e",
                "ViewId": 'form1'
            });
        }


        function iterate(topics, parent, depth, topicCallback, questionCallback) {
            $(topics).each(function () {
                var t = this;
                if (topicCallback)
                    topicCallback(t, parent);
                $(t.questions).each(function () {
                    var q = this;
                    if (questionCallback)
                        questionCallback(q, t, parent, depth);
                });
                if (t.topics)
                    iterate(t.topics, t, depth + 1, topicCallback, questionCallback);
            });
        }

        function populateItems(list, fields, row, callback) {
            var batch = [], batchList = [], unresolvedBatch = [], clearedList = [];
            // scan the list to ensure that DataValueField and DataTextField are defined
            $(list).each(function (index) {
                var f = this;
                if (!f.ItemsDataValueField)
                    f.ItemsDataValueField = _app.cache[f.ItemsDataController + '_' + f.ItemsDataView + '_DataValueField'];
                if (!f.ItemsDataTextField)
                    f.ItemsDataTextField = _app.cache[f.ItemsDataController + '_' + f.ItemsDataView + '_DataTextField'];
                if (!f.ItemsDataValueField || !f.ItemsDataTextField) {
                    unresolvedBatch.push({
                        controller: f.ItemsDataController,
                        view: f.ItemsDataView,
                        requiresData: false,
                        metadataFilter: ['fields'],
                        _fieldIndex: index
                    });
                }
            });
            if (unresolvedBatch.length) {
                busy(true);
                _app.execute({
                    batch: unresolvedBatch,
                    success: function (result) {
                        $(result).each(function (index) {
                            var f = list[unresolvedBatch[index]._fieldIndex],
                                r = this.rawResponse;
                            if (!f.ItemsDataValueField)
                                $(r.Fields).each(function () {
                                    var f2 = this;
                                    if (f2.IsPrimaryKey) {
                                        f.ItemsDataValueField = f2.Name;
                                        _app.cache[f.ItemsDataController + '_' + f.ItemsDataView + '_DataValueField'] = f2.Name;
                                        return false;
                                    }
                                });
                            if (!f.ItemsDataTextField) {
                                f.ItemsDataTextField = r.Fields[0].Name;
                                _app.cache[f.ItemsDataController + '_' + f.ItemsDataView + '_DataTextField'] = f.ItemsDataTextField;
                            }
                        });
                        populateItems(list, fields, row, callback);
                    },
                    error: function (error) {
                        busy(false);
                    }
                });
                return;
            }
            // request item values
            $(list).each(function () {
                var f = this, m,
                    dataView = f._dataView,
                    fieldFilter = [f.ItemsDataValueField, f.ItemsDataTextField],
                    copy = f.Copy,
                    contextFields = f.ContextFields,
                    selectRequest = {
                        controller: f.ItemsDataController,
                        view: f.ItemsDataView,
                        sortExpression: f.ItemsDataTextField,
                        fieldFilter: fieldFilter,
                        metadataFilter: ['fields'],
                        pageSize: 1000,
                        distinct: _app.is(f.Tag, 'lookup-distinct')// !!(f.Tag && f.Tag.match(/\blookup-distinct(?!-none)/))
                    };
                if (copy)
                    while (m = _app._fieldMapRegex.exec(copy))
                        fieldFilter.push(m[2]);
                if (contextFields) {
                    //if (!row)
                    //    row = dataView.survey('row');
                    var filter = [],
                        contextField;
                    while (m = _app._fieldMapRegex.exec(contextFields)) {
                        if (dataView)
                            contextField = dataView.findField(m[2]);
                        else
                            $(fields).each(function () {
                                var f = this;
                                if (f.Name == m[2]) {
                                    contextField = f;
                                    return false;
                                }
                            });
                        var fieldValue = row[contextField.Index],
                            cascadingDependency = !dependsOn(contextField, f);
                        if (/*f.ItemsDataController != contextField.ItemsDataController && */cascadingDependency || fieldValue != null)
                            if (fieldValue == null && cascadingDependency) {
                                f.Items = [];
                                clearedList.push(f);
                            }
                            else if (contextField.ItemsTargetController || contextField.ItemsStyle == 'CheckBoxList') {
                                var list = _app.csv.toArray(fieldValue);
                                if (list.length <= 1)
                                    filter.push({ field: m[1], value: list[0] });
                                else
                                    filter.push({ field: m[1], operator: 'in', values: list });
                            }
                            else
                                filter.push({ field: m[1], value: fieldValue });
                    }
                    if (filter.length)
                        selectRequest.filter = filter;
                }
                if (!f.skipPopulate && clearedList.indexOf(f) == -1) {
                    batch.push(selectRequest);
                    batchList.push(f);
                }

            });
            if (batch.length) {
                busy(true);
                _app.execute({
                    batch: batch,
                    done: function (result) {
                        busy(false);
                        $(batchList).each(function (index) {
                            var f = this,
                                r = batch[index],
                                p = result[index].rawResponse,
                                pageFieldMap = {};
                            $(p.Fields).each(function (index) {
                                pageFieldMap[this.Name] = index;
                            });
                            f.Items = [];
                            $(p.Rows).each(function () {
                                var row = this,
                                    item = [], i;
                                for (i = 0; i < r.fieldFilter.length; i++)
                                    item.push(row[pageFieldMap[r.fieldFilter[i]]]);
                                if (pageFieldMap['group_count_'] != null)
                                    item.push(row[pageFieldMap['group_count_']]);
                                f.Items.push(item);
                            });
                        });
                        if (callback)
                            callback(batchList.concat(clearedList));
                    },
                    fail: function (error) {
                        busy(false);
                    }
                });
            }
            else if (callback && clearedList.length)
                callback(batchList);
        }

        function refresh(callback) {
            var dataView = _touch.dataView(),
                extension = dataView.extension();
            options.compiled = function (result) {
                var form = extension._disposeForm();
                dataView._views[0].Layout = options.layout;
                // replace layout
                var newForm = _touch.createLayout(dataView, _touch.calcWidth(form.parent()));
                newForm = newForm.insertAfter(form);
                _touch.prepareLayout(dataView, result.NewRow, newForm);
                form.remove();
                // refresh internal elements
                extension._skipRefresh = true;
                dataView._pageIndex = -1;
                dataView._editRow = null;
                dataView._onGetPageComplete(result, null);
                extension._skipRefresh = false;
                // state has changed
                extension.stateChanged(false);
                if (callback)
                    callback(newForm);
            };
            compile();
        }

        function register(data, callback) {
            if (!_app.survey.registrations)
                _app.survey.registrations = {};
            var result = _app.survey.registrations[data] != true;
            _app.survey.registrations[data] = true;
            if (result && callback)
                callback();
            return result;
        }

        function dependsOn(childField, masterField) {
            var contextFields = childField.ContextFields,// var iterator = /\s*(\w+)\s*(=\s*(\w+)\s*)?(,|$)/g;
                test = new RegExp('=\\s*' + masterField.Name + '\\s*(,|$)');
            return !!(contextFields && contextFields.match(test));

        }

        function compile() {
            var requiresItems = [],
                fieldMap = {}, fieldIndex = 0,
                context = options.context,
                initValues = context && context._initVals,
                result = {
                    Controller: controller, View: 'form1',
                    TotalRowCount: -1,
                    Fields: [
                        { "Name": "sys_pk_", "Type": "Int32", "Label": "", "IsPrimaryKey": true, "ReadOnly": true, "Hidden": true, "AllowNulls": true, "Columns": 20 }
                    ],
                    Views: [{ Id: 'form1', Label: options.text, Type: 'Form' }],
                    ViewHeaderText: options.description,
                    ViewLayout: options.layout,
                    Expressions: [
                    ],
                    //SupportsCaching: true, IsAuthenticated: true,
                    ActionGroups: [
                        {
                            Scope: 'Form', Id: 'form',
                            Actions: [
                                //{ 'Id': 'a3', 'CommandName': 'Confirm', 'WhenLastCommandName': 'Edit' },
                                //{ 'Id': 'a4', 'CommandName': 'Cancel', 'WhenLastCommandName': 'Edit' },
                                //{ 'Id': 'a5', 'CommandName': 'Edit' }
                            ]
                        }
                    ],
                    Categories: [],
                    NewRow: [1],
                    Rows: []
                },
                buttons = options.buttons;

            function addDynamicExpression(scope, target, test) {
                result.Expressions.push({ Scope: scope, Target: target, Test: test, Type: 1, ViewId: 'form1' });
            }

            if (options.submit) {
                var submitKey = options.submitKey;
                result.ActionGroups[0].Actions.push({ Id: 'submit', CommandName: 'Confirm', WhenLastCommandName: 'New', HeaderText: options.submitText, Confirmation: options.submitConfirmation, Key: submitKey == false ? null : (submitKey || 'Enter'), CssClass: options.submitIcon });
            }
            if (options.cancel != false)
                result.ActionGroups[0].Actions.push({ Id: 'a2', CommandName: 'Cancel', WhenLastCommandName: 'New' });

            ensureTopics(options);
            var index = 0;
            iterate(options.topics, null, 0, function (topic, parent, depth) {
                var categoryIndex = result.Categories.length,
                    categoryVisibleWhen = topic.visibleWhen,
                    category = {
                        "Id": "c" + categoryIndex, "Index": categoryIndex,
                        HeaderText: topic.text, Description: topic.description,
                        Wizard: topic.wizard,
                        Flow: topic.flow == 'newColumn' || (index == 0) ? 'NewColumn' : (topic.flow == 'newRow' ? 'NewRow' : ''),
                        Wrap: topic.wrap != null ? topic.wrap : null,
                        Floating: !!topic.floating,
                        Collapsed: topic.collapsed == true,
                        Tab: topic.tab
                    };
                if (categoryVisibleWhen)
                    addDynamicExpression(2, category.Id, categoryVisibleWhen);
                if (depth > 0)
                    category.Depth = depth;
                result.Categories.push(category);
                topic._categoryIndex = categoryIndex;
                index++;

            }, function (fd, topic, parent, depth) {
                var fdType = fd.type || 'String',
                    fdFormat = fd.format || fd.dataFormatString,
                    fdMode = fd.mode,
                    fdColumns = fd.columns,
                    fdRows = fd.rows,
                    fdOptions = fd.options,
                    fdTags = fdOptions ? _app.toTags(fdOptions) : fd.tags,
                    items = fd.items,
                    fdValue = fd.value,
                    fdContext = fd.context,
                    fdName = fd.name,
                    fdVisibleWhen = fd.visibleWhen,
                    fdReadOnlyWhen = fd.readOnlyWhen,
                    fdTooltip = fd.tooltip,
                    f = {
                        Name: fdName, HtmlEncode: true,
                        AllowNulls: fd.required != true,
                        Label: fd.text == false ? '&nbsp;' : fd.text || fd.label || prettyText(fd.name),
                        Hidden: fd.hidden == true,
                        CausesCalculate: fd.causesCalculate == true
                    };
                if (!fdName) return;
                if (initValues && fdName in initValues)
                    fdValue = initValues[fdName];
                if (fd.causesCalculate)
                    f.CausesCalculate = true;
                switch (fdType.toLowerCase()) {
                    case 'text':
                    case 'string':
                        fdType = 'String';
                        break;
                    case 'date':
                        fdType = 'DateTime';
                        if (!fdFormat) {
                            fdFormat = 'd';
                            if (fdColumns == null)
                                fdColumns = 10;
                        }
                        break;
                    case 'datetime':
                        fdType = 'DateTime';
                        if (fdColumns == null)
                            fdColumns = 20;
                        break;
                    case 'time':
                        fdType = 'DateTime';
                        if (!fdFormat) {
                            fdFormat = 't';
                            if (fdColumns == null)
                                fdColumns = 8;
                        }
                        break;
                    case 'number':
                        fdType = 'Double';
                        break;
                    case 'int':
                        fdType = 'Int32';
                        break;
                    case 'bool':
                    case 'Boolean':
                        fdType = 'Boolean';
                        if (!items && fd.required) {
                            items = { style: 'CheckBox' };
                            if (fdValue == null)
                                fdValue = false;
                        }
                        break;
                    case 'money':
                        fdType = 'Currency';
                        break;
                    case 'memo':
                        fdType = 'String';
                        if (fdRows == null)
                            fdRows = 5;
                        break;
                    case 'blob':
                        var x = {
                            "Name": "Picture",
                            "Type": "Byte[]",
                            "Label": "Picture",
                            "AllowQBE": false,
                            "AllowSorting": false,
                            "SourceFields": "CategoryID",
                            "AllowNulls": true,
                            "Columns": 15,
                            "OnDemand": true,
                            "OnDemandHandler": "CategoriesPicture",
                            "ShowInSummary": true
                        };
                        fdType = 'Byte[]';
                        f.OnDemand = true;
                        break;
                }
                f.Type = fdType;
                if (fdType == 'String')
                    f.Len = fd.length || 100;
                if (fdType == 'DateTime' && !fdFormat)
                    fdFormat = 'g';
                if (fdType == 'Currency' && !fdFormat)
                    fdFormat = 'c';
                if (fdFormat)
                    if (typeof fdFormat == 'string')
                        f.DataFormatString = fdFormat;
                    else {
                        var fmt = fdFormat.DataFormatString;
                        if (fmt) {
                            f.DataFormatString = fmt;
                            if (fdType == 'DateTime') {
                                f.TimeFmtStr = fdFormat.TimeFmtStr;
                                f.DateFmtStr = fdFormat.DateFmtStr;
                            }
                        }
                        delete fd.format;
                    }
                if (fdColumns)
                    f.Columns = fdColumns;
                if (fdRows) {
                    f.Rows = fdRows;
                    if (fdType == 'String' && fdRows > 1)
                        f.Len = 0;
                }
                if (fd.placeholder)
                    f.Watermark = fd.placeholder;
                if (fdTags)
                    f.Tag = typeof fdTags === 'string' ? fdTags : fdTags.join(',');
                if (fdContext) {
                    if (typeof fdContext !== 'string') {
                        fdContext.forEach(function (s, index) {
                            if (!s.match(/=/))
                                fdContext[index] = s + '=' + s;
                        });
                        fdContext = fdContext.join(',');
                    }
                    f.ContextFields = fdContext;
                }
                if (fd.htmlEncode == false)
                    f.HtmlEncode = false;

                if (fdMode) {
                    if (fdMode == 'password')
                        f.TextMode = 1;
                    if (fdMode == 'rtf')
                        f.TextMode = 2;
                    if (fdMode == 'static')
                        f.TextMode = 4;
                }

                if (options.readOnly || fd.readOnly && typeof fd.readOnly != 'function')
                    f.ReadOnly = true;

                if (!f.Hidden)
                    f.CategoryIndex = topic._categoryIndex;
                if (fd.extended)
                    f.Extended = fd.extended;
                if (fd.altText)
                    f.AltHeaderText = fd.altText;
                if (fd.footer)
                    f.FooterText = fd.footer;
                if (fdTooltip)
                    f.ToolTip = fdTooltip;

                var copy = items && items.copy;
                if (copy) {
                    if (typeof copy !== 'string') {
                        copy = [];
                        $(items.copy).each(function () {
                            var copyInfo = this;
                            copy.push(copyInfo.to + '=' + copyInfo.from);
                        });
                        copy = copy.join('\n');
                    }
                    f.Copy = copy;
                }
                var filter = items && items.filter;
                if (filter) {
                    if (typeof filter !== 'string') {
                        filter = [];
                        $(items.filter).each(function () {
                            var filterInfo = this;
                            filter.push(filterInfo.match + '=' + filterInfo.to);
                        });
                        filter = filter.join(',');
                    }
                    f.ContextFields = filter;
                }

                if (items) {
                    var itemsList = items.values || items.list;
                    itemsStyle = items.style || (items.list ? 'DropDownList' : 'Lookup');
                    f.Items = [];
                    if (_isTagged(fdTags, 'lookup-auto-complete-anywhere'))
                        f.SearchOptions = '$autocompleteanywhere';
                    if (itemsStyle == 'Lookup' && _isTagged(fdTags, 'lookup-distinct'))
                        itemsStyle = 'AutoComplete';
                    if (itemsStyle.match(/AutoComplete|Lookup|DropDown/) && _isTagged(fdTags, 'lookup-multiple')) {
                        f.ItemsTargetController = '_basket';
                        if (itemsStyle == 'AutoComplete' && fdValue != null) {
                            if (items.dataValueField == items.dataTextField) {
                                if (typeof fdValue === 'string')
                                    fdValue = _app.csv.toArray(fdValue);
                                $(fdValue).each(function () {
                                    var v = this;
                                    f.Items.push([v, v, null]);
                                });
                            }
                            else
                                requiresItems.push(f);
                        }
                    }
                    if (fdValue != null && (f.ItemsTargetController || itemsStyle == 'CheckBoxList')) {
                        if (Array.isArray(fdValue))
                            fdValue = _app.csv.toString(fdValue);
                        else if (typeof fdValue !== 'string')
                            fdValue = fdValue.toString();
                    }
                    //if (_app.read(fd, 'options.lookup.distinct'))
                    //    f.DistinctValues = true;
                    if (items.list) {
                        $(items.list).each(function () {
                            var item = this, v = item.value, t = item.text, c = item.count,
                                newItem = [v, t == null ? v : t];
                            if (c != null)
                                newItem.push(c);
                            f.Items.push(newItem);
                        });
                    }
                    else if (items.controller) {
                        f.ItemsDataController = items.controller;
                        f.ItemsDataView = items.view || 'grid1',
                            f.ItemsDataValueField = items.dataValueField;
                        f.ItemsDataTextField = items.dataTextField;
                        f.ItemsNewDataView = items.newView;
                        if (!itemsStyle.match(/AutoComplete|Lookup/))
                            requiresItems.push(f);
                        if (items.dataValueField != items.dataTextField)
                            f._autoAlias = true;
                    }
                    f.ItemsStyle = itemsStyle;
                    if (items.disabled)
                        f.ItemsStyle = null;
                }
                if (fdVisibleWhen)
                    //result.Expressions.push({ Scope: 3, Target: fd.name, Test: fdVisibleWhen, Type: 1, ViewId: 'form1' });
                    addDynamicExpression(3, fdName, fdVisibleWhen);
                if (fdReadOnlyWhen)
                    //result.Expressions.push({ Scope: 5, Target: fd.name, Test: fdReadOnlyWhen, Type: 1, ViewId: 'form1' });
                    addDynamicExpression(5, fdName, fdReadOnlyWhen, 5);
                result.Fields.push(f);
                fieldMap[f.Name] = f;
                if (typeof fdValue !== 'function')
                    result.NewRow[result.Fields.length - 1] = fdValue;
            });

            result.Fields.forEach(function (f) {
                var contextFields = f.ContextFields;
                f.Index = fieldIndex++;
                if (contextFields)
                    $(contextFields.split(_app._simpleListRegex)).each(function () {
                        var cm = this.split(_app._fieldMapRegex),
                            cf = cm ? fieldMap[cm[2]] : null;
                        if (cf && cf.ItemsDataController == f.ItemsDataController) {
                            f.requiresDynamicNullItem = true;
                            return false;
                        }
                    });
            });

            if (options.init)
                createRule(result.Expressions, 'init', options.init, 'New', 'form1', 'After');
            if (options.submit)
                createRule(result.Expressions, 'submit', options.submit, 'Confirm', null, 'Before');
            if (options.cancel)
                createRule(result.Expressions, 'cancel', options.cancel, 'Cancel', null, 'Before');
            if (options.calculate)
                createRule(result.Expressions, 'calculate', options.calculate, 'Calculate', null, 'Execute');

            // create actions and matching business rules from buttons
            if (buttons) {
                var actionGroupMap = { form: result.ActionGroups[0] };
                options._handlers = {};
                $(buttons).each(function () {
                    var btn = this,
                        scope = btn.scope,
                        group, action;
                    if (!scope)
                        scope = 'form';
                    group = actionGroupMap[scope];
                    if (!btn.text)
                        group.Actions.push({ Id: 'div' + group.Actions.length, CommandName: btn.id, WhenLastCommandName: 'New' });
                    else {
                        action = { Id: btn.id, CommandName: btn.id, WhenLastCommandName: 'New', HeaderText: btn.text, Key: btn.key };
                        if (btn.icon)
                            action.CssClass = btn.icon;
                        if (btn.when)
                            action.WhenClientScript = typeof btn.when == 'function' ? btn.when :
                                function () {
                                    var e = $.Event(btn.when, { dataView: this, argument: btn.argument });
                                    $(document).trigger(e);
                                    return !e.isDefaultPrevented();
                                };
                        if (!group) {
                            group = { Scope: scope[0].toUpperCase() + scope.substring(1), Id: scope, Actions: [] };
                            result.ActionGroups.push(group);
                            actionGroupMap[scope] = group;
                        }
                        if (scope == 'form')
                            group.Actions.splice(group.Actions.length - 1, 0, action);
                        else
                            group.Actions.push(action);
                        if (typeof btn.click == 'function') {
                            options._handlers[btn.id] = function (e) {
                                e.preventDefault();
                                btn.click.call(e.dataView, e);
                            }
                        }
                        createRule(result.Expressions, '_handlers.' + btn.id, btn.click, btn.id, null, 'Execute', btn.argument);
                    }
                });
            }

            result.Tag = (options.tags || '') + ' ignore-unsaved-changes';
            if (result.Fields.length == 1)
                result.Fields[0].Hidden = false;
            var compileCallback = options.compiled;
            if (compileCallback) {
                if (requiresItems.length)
                    populateItems(requiresItems, result.Fields, result.NewRow, function () {
                        compileCallback(result);
                    });
                else
                    compileCallback(result);
                options.compiled = null;
            }
            return result;
        }

        if (method == 'show')
            if (options.external) {
                var survey = _app.surveyLibrary[controller];
                if (survey)
                    show(survey);
                else {
                    busy(true);
                    var dataView = findDataView(options.parent);
                    // load the survey from the server


                    function failedToLoad(result) {
                        busy(false);
                        var create = options.create;
                        if (create)
                            if (typeof create == 'string')
                                $(document).trigger($.Event(create, { survey: options }));
                            else
                                create.call(options);
                        else
                            _app.alert('Unable to load survey ' + controller + ' from the server.');
                    }

                    if (options.tryLoad == false || _app.surveyFailedToLoad[controller])
                        failedToLoad({});
                    else
                        $.ajax({
                            //url:  toUrl(originalControllerName + '.js'),// dataView.get_baseUrl() + '/scripts/surveys/' + originalControllerName + '.js',
                            //dataType: 'text',
                            //cache: false
                            url: __servicePath + '/GetSurvey',
                            data: JSON.stringify({ name: originalControllerName }),
                            method: 'POST',
                            cache: false
                        }).done(function (result) {
                            busy(false);
                            result = result.d;
                            if (typeof result == 'string') {
                                _app.surveyLibrary[controller] = result;
                                show(result);
                            }
                            else {
                                failedToLoad(result);
                                _app.surveyFailedToLoad[controller] = true;
                            }
                        }).fail(failedToLoad);
                }
            }
            else
                showCompiled(options);
        else if (method == 'compile')
            // produce an emulation of the server response for a controller and call GetPageComplete with the result
            return compile();
        else if (method == 'populateItems') {
            var fieldWithContext = [],
                dataView = options.dataView;
            $(dataView._allFields).each(function () {
                var f = this;
                if (f.ItemsAreDynamic && f.ContextFields && !f.skipPopulate)
                    fieldWithContext.push(f);
            });
            if (fieldWithContext.length)
                populateItems(fieldWithContext, dataView._allFields, dataView.row(), options.callback);
        }
        else if (method == 'refresh')
            refresh(arguments[2]);
        else if (method == 'register')
            return register(arguments[1], arguments[2]);
        else
            _app.alert('Unsupported survey method: ' + method);
    }

    //
    // Survey: Batch Edit 
    // 

    $(document).on('beforetouchinit.app', function () {
        _touch = _app.touch;
    }).on('batcheditsubmit.dataview.app', function (e) {
        var rules = e.rules;
        if (rules.busy()) {
            rules.preventDefault();
            return;
        }
        var dataView = rules.dataView(),
            row = dataView.row(),
            surveyContext = dataView.survey().context,
            values = [],
            focusField;
        $(dataView._allFields).each(function () {
            var f = this,
                fieldName = f.Name.match(/^(.+?)\_BatchEdit$/),
                bf, dependency, v;
            if (fieldName && f.Type == 'Boolean') {
                bf = dataView.findField(f.Name);
                if (bf && row[bf.Index]) {
                    bf = dataView.findField(fieldName[1]);
                    if (bf)
                        v = row[bf.Index];
                    if (bf.Extended && !bf.Extended.allowNulls && v == null) {
                        focusField = bf;
                        return false;
                    }
                    else {
                        dependency = bf.Extended.dependency;
                        if (dependency) {
                            $(dependency).each(function () {
                                var fi = this,
                                    pf = dataView.findField(fi.Name),
                                    pv = row[pf.Index];
                                if (v == null & pv != null) {
                                    focusField = bf;
                                    return false;
                                }
                                values.push({ name: pf.Name, newValue: pv });
                            });
                        }
                        if (focusField)
                            return false;
                        values.push({ name: bf.Name, newValue: v });
                    }
                }
            }
        });
        if (focusField) {
            rules.preventDefault();
            rules.result.focus(focusField.Name, resourcesValidator.RequiredField);
        }
        else if (values.length) {
            rules.preventDefault();
            busy(true);
            var parentDataView = dataView.get_parentDataView();
            $(parentDataView._keyFields).each(function () {
                var pk = this;
                values.push({ name: pk.Name });
            });
            _app.confirm(resourcesWhenLastCommandBatchEdit.Confirmation).then(function () {
                _app.execute({
                    controller: surveyContext.controller, view: surveyContext.view, command: 'Update', lastCommand: 'BatchEdit',
                    values: values, selectedKeys: parentDataView._selectedKeyList,
                    error: function () {
                        busy(false);
                    }
                }).done(function (result) {
                    busy(false);

                    function clearSelectionInParentDataView() {
                        parentDataView._clearSelectedKey();
                        parentDataView._selectedKeyList = [];
                        parentDataView.sync();
                    }

                    if (_touch) {
                        _touch.pageShown(function () {
                            //parentDataView._keepKeyList = true; -- multiple requests where to unselect the checkboxes.
                            _touch.notify(String.format(resourcesMobile.BatchEdited, result.rowsAffected));
                            clearSelectionInParentDataView();
                        });
                        dataView.cancel();
                    }
                    else {
                        dataView.cancel();
                        parentDataView.set_selectedValue('');
                        clearSelectionInParentDataView();
                    }
                });
            });
        }
    });

    if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

    /*!
    * Signature Pad v1.3.5
    * https://github.com/szimek/signature_pad
    *
    * Copyright 2015 Szymon Nowak
    * Released under the MIT license
    *
    * The main idea and some parts of the code (e.g. drawing variable width Bézier curve) are taken from:
    * http://corner.squareup.com/2012/07/smoother-signatures.html
    *
    * Implementation of interpolation using cubic Bézier curves is taken from:
    * http://benknowscode.wordpress.com/2012/09/14/path-interpolation-using-cubic-bezier-and-control-point-estimation-in-javascript
    *
    * Algorithm for approximated length of a Bézier curve is taken from:
    * http://www.lemoda.net/maths/bezier-length/index.html
    *
    */
    _window.SignaturePad = (function (document) {

        var SignaturePad = function (canvas, options) {
            var self = this,
                opts = options || {};

            this.velocityFilterWeight = opts.velocityFilterWeight || 0.7;
            this.minWidth = opts.minWidth || 0.5;
            this.maxWidth = opts.maxWidth || 2.5;
            this.dotSize = opts.dotSize || function () {
                return (this.minWidth + this.maxWidth) / 2;
            };
            this.penColor = opts.penColor || "black";
            this.backgroundColor = opts.backgroundColor || "rgba(0,0,0,0)";
            this.onEnd = opts.onEnd;
            this.onBegin = opts.onBegin;

            this._canvas = canvas;
            this._ctx = canvas.getContext("2d");
            this.clear();

            this._handleMouseEvents();
            this._handleTouchEvents();
        };

        SignaturePad.prototype.clear = function () {
            var ctx = this._ctx,
                canvas = this._canvas;

            ctx.fillStyle = this.backgroundColor;
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            this._reset();
        };

        SignaturePad.prototype.toDataURL = function (imageType, quality) {
            var canvas = this._canvas;
            return canvas.toDataURL.apply(canvas, arguments);
        };

        SignaturePad.prototype.fromDataURL = function (dataUrl) {
            var self = this,
                image = new Image(),
                ratio = window.devicePixelRatio || 1,
                width = this._canvas.width / ratio,
                height = this._canvas.height / ratio;

            this._reset();
            image.src = dataUrl;
            image.onload = function () {
                self._ctx.drawImage(image, 0, 0, width, height);
            };
            this._isEmpty = false;
        };

        SignaturePad.prototype._strokeUpdate = function (event) {
            var point = this._createPoint(event);
            this._addPoint(point);
        };

        SignaturePad.prototype._strokeBegin = function (event) {
            var ctx = this._ctx,
                canvas = this._canvas;

            // clear the canvas to remove "Sign Here" watermark - Code On Time
            if (this._isEmpty) {
                ctx.fillStyle = this.backgroundColor;
                ctx.fillRect(0, 0, canvas.width, canvas.height);
                ctx.fillStyle = this.penColor;
            }

            this._reset();
            this._strokeUpdate(event);
            if (typeof this.onBegin === 'function') {
                this.onBegin(event);
            }
        };

        SignaturePad.prototype._strokeDraw = function (point) {
            var ctx = this._ctx,
                dotSize = typeof (this.dotSize) === 'function' ? this.dotSize() : this.dotSize;

            ctx.beginPath();
            this._drawPoint(point.x, point.y, dotSize);
            ctx.closePath();
            ctx.fill();
        };

        SignaturePad.prototype._strokeEnd = function (event) {
            var canDrawCurve = this.points.length > 2,
                point = this.points[0];

            if (!canDrawCurve && point) {
                this._strokeDraw(point);
            }
            if (typeof this.onEnd === 'function') {
                this.onEnd(event);
            }
        };

        SignaturePad.prototype._handleMouseEvents = function () {
            var self = this,
                canvas = $(self._canvas);
            this._mouseButtonDown = false;

            // using jQuery for event handling to simplify cleanup of resources - Code On Time

            canvas.on("mousedown", function (event) {
                event = event.originalEvent;
                if (event.which === 1) {
                    self._mouseButtonDown = true;
                    self._strokeBegin(event);
                }
            });

            canvas.on("mousemove", function (event) {
                event = event.originalEvent;
                if (self._mouseButtonDown) {
                    self._strokeUpdate(event);
                }
            });

            $(document).on("mouseup", function (event) {
                event = event.originalEvent;
                if (event.which === 1 && self._mouseButtonDown) {
                    self._mouseButtonDown = false;
                    self._strokeEnd(event);
                }
            });
        };

        SignaturePad.prototype._handleTouchEvents = function () {
            var self = this,
                canvas = $(self._canvas);

            // Pass touch events to canvas element on mobile IE.
            this._canvas.style.msTouchAction = 'none';

            // using jQuery for event handling to simplify cleanup of resources - Code On Time

            canvas.on("touchstart", function (event) {
                event = event.originalEvent;
                var touch = event.changedTouches[0];
                self._strokeBegin(touch);
            });

            canvas.on("touchmove", function (event) {
                event = event.originalEvent;
                // Prevent scrolling.
                event.preventDefault();

                var touch = event.changedTouches[0];
                self._strokeUpdate(touch);
            });

            $(document).on("touchend", function (event) {
                event = event.originalEvent;
                var wasCanvasTouched = event.target === self._canvas;
                if (wasCanvasTouched) {
                    self._strokeEnd(event);
                }
            });
        };

        SignaturePad.prototype.isEmpty = function () {
            return this._isEmpty;
        };

        SignaturePad.prototype._reset = function () {
            this.points = [];
            this._lastVelocity = 0;
            this._lastWidth = (this.minWidth + this.maxWidth) / 2;
            this._isEmpty = true;
            this._ctx.fillStyle = this.penColor;
        };

        SignaturePad.prototype._createPoint = function (event) {
            var rect = this._canvas.getBoundingClientRect();
            return new Point(
                event.clientX - rect.left,
                event.clientY - rect.top
            );
        };

        SignaturePad.prototype._addPoint = function (point) {
            var points = this.points,
                c2, c3,
                curve, tmp;

            points.push(point);

            if (points.length > 2) {
                // To reduce the initial lag make it work with 3 points
                // by copying the first point to the beginning.
                if (points.length === 3) points.unshift(points[0]);

                tmp = this._calculateCurveControlPoints(points[0], points[1], points[2]);
                c2 = tmp.c2;
                tmp = this._calculateCurveControlPoints(points[1], points[2], points[3]);
                c3 = tmp.c1;
                curve = new Bezier(points[1], c2, c3, points[2]);
                this._addCurve(curve);

                // Remove the first element from the list,
                // so that we always have no more than 4 points in points array.
                points.shift();
            }
        };

        SignaturePad.prototype._calculateCurveControlPoints = function (s1, s2, s3) {
            var dx1 = s1.x - s2.x, dy1 = s1.y - s2.y,
                dx2 = s2.x - s3.x, dy2 = s2.y - s3.y,

                m1 = { x: (s1.x + s2.x) / 2.0, y: (s1.y + s2.y) / 2.0 },
                m2 = { x: (s2.x + s3.x) / 2.0, y: (s2.y + s3.y) / 2.0 },

                l1 = Math.sqrt(dx1 * dx1 + dy1 * dy1),
                l2 = Math.sqrt(dx2 * dx2 + dy2 * dy2),

                dxm = (m1.x - m2.x),
                dym = (m1.y - m2.y),

                k = l2 / (l1 + l2),
                cm = { x: m2.x + dxm * k, y: m2.y + dym * k },

                tx = s2.x - cm.x,
                ty = s2.y - cm.y;

            return {
                c1: new Point(m1.x + tx, m1.y + ty),
                c2: new Point(m2.x + tx, m2.y + ty)
            };
        };

        SignaturePad.prototype._addCurve = function (curve) {
            var startPoint = curve.startPoint,
                endPoint = curve.endPoint,
                velocity, newWidth;

            velocity = endPoint.velocityFrom(startPoint);
            velocity = this.velocityFilterWeight * velocity
                + (1 - this.velocityFilterWeight) * this._lastVelocity;

            newWidth = this._strokeWidth(velocity);
            this._drawCurve(curve, this._lastWidth, newWidth);

            this._lastVelocity = velocity;
            this._lastWidth = newWidth;
        };

        SignaturePad.prototype._drawPoint = function (x, y, size) {
            var ctx = this._ctx;

            ctx.moveTo(x, y);
            ctx.arc(x, y, size, 0, 2 * Math.PI, false);
            this._isEmpty = false;
        };

        SignaturePad.prototype._drawCurve = function (curve, startWidth, endWidth) {
            var ctx = this._ctx,
                widthDelta = endWidth - startWidth,
                drawSteps, width, i, t, tt, ttt, u, uu, uuu, x, y;

            drawSteps = Math.floor(curve.length());
            ctx.beginPath();
            for (i = 0; i < drawSteps; i++) {
                // Calculate the Bezier (x, y) coordinate for this step.
                t = i / drawSteps;
                tt = t * t;
                ttt = tt * t;
                u = 1 - t;
                uu = u * u;
                uuu = uu * u;

                x = uuu * curve.startPoint.x;
                x += 3 * uu * t * curve.control1.x;
                x += 3 * u * tt * curve.control2.x;
                x += ttt * curve.endPoint.x;

                y = uuu * curve.startPoint.y;
                y += 3 * uu * t * curve.control1.y;
                y += 3 * u * tt * curve.control2.y;
                y += ttt * curve.endPoint.y;

                width = startWidth + ttt * widthDelta;
                this._drawPoint(x, y, width);
            }
            ctx.closePath();
            ctx.fill();
        };

        SignaturePad.prototype._strokeWidth = function (velocity) {
            return Math.max(this.maxWidth / (velocity + 1), this.minWidth);
        };


        var Point = function (x, y, time) {
            this.x = x;
            this.y = y;
            this.time = time || new Date().getTime();
        };

        Point.prototype.velocityFrom = function (start) {
            return (this.time !== start.time) ? this.distanceTo(start) / (this.time - start.time) : 1;
        };

        Point.prototype.distanceTo = function (start) {
            return Math.sqrt(Math.pow(this.x - start.x, 2) + Math.pow(this.y - start.y, 2));
        };

        var Bezier = function (startPoint, control1, control2, endPoint) {
            this.startPoint = startPoint;
            this.control1 = control1;
            this.control2 = control2;
            this.endPoint = endPoint;
        };

        // Returns approximated length.
        Bezier.prototype.length = function () {
            var steps = 10,
                length = 0,
                i, t, cx, cy, px, py, xdiff, ydiff;

            for (i = 0; i <= steps; i++) {
                t = i / steps;
                cx = this._point(t, this.startPoint.x, this.control1.x, this.control2.x, this.endPoint.x);
                cy = this._point(t, this.startPoint.y, this.control1.y, this.control2.y, this.endPoint.y);
                if (i > 0) {
                    xdiff = cx - px;
                    ydiff = cy - py;
                    length += Math.sqrt(xdiff * xdiff + ydiff * ydiff);
                }
                px = cx;
                py = cy;
            }
            return length;
        };

        Bezier.prototype._point = function (t, start, c1, c2, end) {
            return start * (1.0 - t) * (1.0 - t) * (1.0 - t)
                + 3.0 * c1 * (1.0 - t) * (1.0 - t) * t
                + 3.0 * c2 * (1.0 - t) * t * t
                + end * t * t * t;
        };

        return SignaturePad;
    })(document);

    _app.storage = {
        set: function (name, value) {
            var ls = _window.localStorage;
            if (ls)
                try {
                    if (name.match(/\*$/)) {
                        name = name.substring(0, name.length - 1);
                        var keyList = [], key, k, v;
                        for (key in ls)
                            if (key.indexOf(name) == 0)
                                keyList.push(key);
                        value = JSON.parse(value);
                        keyList.forEach(function (key) {
                            for (k in value)
                                if (key.endsWith('_' + k)) {
                                    v = value[k];
                                    if (v == null)
                                        ls.removeItem(key);
                                    else
                                        ls[key] = v;
                                    break;
                                }
                        });
                    }
                    else if (value == null)
                        ls.removeItem(name);
                    else
                        ls[name] = value;
                } catch (ex) {
                }
        },
        get: function (name) {
            var ls = _window.localStorage;
            if (ls)
                try {
                    return ls[name];
                }
                catch (ex) {
                    return null;
                }
            else
                return null;
        },
        remove: function (name) {
            this.set(name, null);
        },
        lock: function () {
        },
        unlock: function () {
        }
    }

    // account manager
    _app.AccountManager = {
        enabled: function () {
            return _touch && _touch.settings('membership.enabled') != false && _touch.settings('membership.accountManager.enabled') != false;
        },
        set: function (user) {
            // debug
            //if (user.UserName.toLowerCase() == 'user3')
            //    user.Picture = 'http://www.sephora.com/contentimages/categories/makeup/CONTOURING/030515/animations/round/round_01_before.jpg';
            //if (user.UserName.toLowerCase() == 'admin')
            //    user.Picture = 'https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcRUZY8AMLAo6PiDmom81fM8lD0-tClyVVdHs4Q1EYbIU_SfcF6t';
            //if (user.UserName.toLowerCase() == 'user')
            //    user.Picture = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQMXzI3JlABWGZvNietVb-wFvLRtRY8U_pyfwnFdgycScK_RjGG';
            // end debug
            if (this.enabled()) {
                var identities = this.list();
                identities[user.UserName] = user;
                identities._lastUser = user.UserName;
                //_window.localStorage.setItem('identities', JSON.stringify(identities));
                _app.storage.set('identities', JSON.stringify(identities));
            }
            else {
                if (user.Picture) {
                    var pictureMap = _app.storage.get('userProfilePictures');//storage['userProfilePictures'];
                    if (pictureMap)
                        pictureMap = JSON.parse(pictureMap);
                    else
                        pictureMap = {};
                    this._pictureMap = pictureMap;
                    pictureMap[user.UserName] = user.Picture;
                    //storage['userProfilePictures'] = JSON.stringify(pictureMap);
                    _app.storage.set('userProfilePictures', JSON.stringify(pictureMap));
                }
            }
        },
        count: function () {
            var list = this.list(),
                count = 0;
            for (var k in list)
                if (k != '_lastUser')
                    count++;

            return count;
        },
        list: function () {
            if (_touch) {
                var identities = this._identities;
                if (!identities) {
                    identities = _app.storage.get('identities');// _window.localStorage.getItem('identities');
                    this._identities = identities = identities ? JSON.parse(identities) : {};
                }
                return identities;
            }
            return {};
        },
        remove: function (username, forget) {
            if (_touch) {
                var identities = this.list();
                if (identities.hasOwnProperty(username))
                    if (forget)
                        delete identities[username];
                    else
                        identities[username].Token = null;
                delete identities._lastUser;
                //_window.localStorage.setItem('identities', JSON.stringify(identities));
                _app.storage.set('identities', JSON.stringify(identities));
            }
        },
        avatar: function (user, icon) {
            var picture;
            if (this.enabled()) {
                var identities = this.list(),
                    identity,
                    picture;
                if (identities) {
                    identity = identities[user],
                        picture = identity && identity.Picture;
                }
            }
            else {
                var pictureMap = this._pictureMap;
                if (!pictureMap) {
                    var profilePictures = _app.storage.get('userProfilePictures');//storage['userProfilePictures'];
                    pictureMap = this._pictureMap = profilePictures ? JSON.parse(profilePictures) : {};
                }
                if (pictureMap)
                    picture = pictureMap[user];
            }
            if (picture)
                icon.css('background-image', 'url("' + picture + '")').parent().addClass('app-has-avatar-with-picture');
        }
    };
})();
/* test */
