(function ($) {
    $.widget("roicp.treeview", {
        mySelf: null,
        executeExpandPaths: false,
        countExpandPathsItens: 0,
        testCounter:0,

        options: {
            source: null,
            pathsToExpand: null,

            // callbacks
            onCompleted: null,
            onNodeExpanded: null,
            onNodeColapsed: null,
            onNodeCreated: null
        },

        _create: function () {
            mySelf = this;
            executeExpandPaths = true;
            countExpandPathsItens = 0;
            testCounter = 0;

            mySelf._countExpandPathsItens(mySelf.options.pathsToExpand);
            mySelf._getChildrenNodes(0, mySelf.element, mySelf._getChildrenNodesCompleted);
        },

        _getChildrenNodes: function (itemId, baseElement, funcCallback) {
            if (typeof mySelf.options.source === "string") {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mySelf.options.source,
                    data: "{ 'upperId':'" + itemId + "' }",
                    dataType: "json",
                    success: function (data) {
                        funcCallback(itemId, baseElement, data);
                    },
                    error: function () {
                        alert("An error occurred when trying to obtain the nodes");
                    }
                });
            };
        },

        _getChildrenNodesCompleted: function (itemId, parentElement, data) {
            if (data != null && data.length > 0) {
                var childTreeTag = $("<ul />");
                childTreeTag.addClass("treeview");

                mySelf._createNode(data, childTreeTag);

                childTreeTag.appendTo(parentElement);

                mySelf._trigger("onCompleted", null, {});

                if (executeExpandPaths) {
                    mySelf._expandNodesInExpandPaths(mySelf.options.pathsToExpand);
                }
            }
        },

        _expandNode: function (itemNodeId) {
            var currentNode = mySelf.element.find("li[id='" + itemNodeId + "']").first();

            mySelf._switchCssClass(currentNode.children(".scontainer-expand"));

            mySelf._getChildrenNodes(itemNodeId, currentNode, mySelf._getChildrenNodesCompleted);

            mySelf._trigger("onNodeExpanded", null, {});
        },

        _collapseNode: function (itemNodeId) {
            var currentNode = mySelf.element.find("li[id='" + itemNodeId + "']").first();

            mySelf._switchCssClass(currentNode.children(".scontainer-collapse"));

            currentNode.children("ul").remove();

            mySelf._trigger("onNodeColapsed", null, {});
        },

        _createNode: function (dataSource, parentElement) {
            for (var i = 0; i < dataSource.length; i++) {
                var item = dataSource[i];
                var hitPosition;

                if (dataSource.length == 1) {
                    hitPosition = "hit-single";
                } else {
                    switch (i) {
                        case 0:
                            hitPosition = "hit-first";
                            break;
                        case dataSource.length - 1:
                            hitPosition = "hit-last";
                            break;
                        default:
                            hitPosition = "";
                            break;
                    }
                }

                // Create the base node li tag
                var nodeInnerTag = $("<li />");
                nodeInnerTag.attr("id", item.Id);
                nodeInnerTag.addClass("node-bg-vimage");
                nodeInnerTag.addClass("without-child-node");

                if (item.HasChild) {
                    nodeInnerTag.removeClass("without-child-node").addClass("with-child-node");
                }

                if (hitPosition == "hit-last" || hitPosition == "hit-single") {
                    nodeInnerTag.removeClass("node-bg-vimage");
                }

                // Create the span hit area (where the plus and minus sign appears)
                var nodeSpanHit = $("<span />");
                nodeSpanHit.attr("id", item.Id);
                nodeSpanHit.addClass("no-hit-area");

                if (item.HasChild) {
                    nodeSpanHit.removeClass("no-hit-area").addClass("scontainer-expand").addClass("hit-area");

                    nodeSpanHit.bind("click", function () {
                        executeExpandPaths = false;
                        mySelf._expandNode($(this).attr("id"));
                    });
                }

                nodeSpanHit.addClass(hitPosition);
                nodeSpanHit.appendTo(nodeInnerTag);

                // Create a span to be used as a render node container.
                // The content of this container could be overridden by a custom _renderItem method.
                var spanText = $("<span />");
                spanText.addClass("scontainer-node-render-container");
                spanText.data("treeview-render-container-item", item);
                mySelf._renderItem(spanText, item);
                spanText.appendTo(nodeInnerTag);

                // Attaching the new node to the parent element
                nodeInnerTag.appendTo(parentElement);

                mySelf._trigger("onNodeCreated", null, {});
            }
        },

        _renderItem: function (spanText, item) {
            var spanName = $("<span />");
            spanName.addClass("scontainer-node-text");
            spanName.html(item.Name);
            spanName.appendTo(spanText);
        },

        _switchCssClass: function (workNode) {
            workNode.unbind('click');

            if (workNode.hasClass("scontainer-collapse")) {
                workNode.removeClass("scontainer-collapse").addClass("scontainer-expand");

                workNode.bind("click", function () {
                    executeExpandPaths = false;
                    mySelf._expandNode($(this).attr("id"));
                });
            } else {
                if (workNode.hasClass("scontainer-expand")) {
                    workNode.removeClass("scontainer-expand").addClass("scontainer-collapse");

                    workNode.bind("click", function () {
                        mySelf._collapseNode($(this).attr("id"));
                    });
                }
            }
        },

        _countExpandPathsItens: function (itens) {
            if ($.isArray(itens)) {
                for (var i = 0; i < itens.length; i++) {
                    countExpandPathsItens += itens[i].length;
                }
            }
        },

        _expandNodesInExpandPaths: function (itens) {
            if ($.isArray(itens)) {
                for (var i = 0; i < itens.length; i++) {
                    var currentItem = itens[i];

                    if ($.isArray(currentItem)) {
                        mySelf._expandNodesInExpandPaths(currentItem);
                    } else {
                        var lastItem = $(itens).last()[0];
                        var currentNode = mySelf.element.find("li[id='" + currentItem + "']").first();

                        if (lastItem != currentItem) {
                            if (currentNode.children(".scontainer-expand").length > 0) {
                                mySelf._expandNode(currentItem);
                            }
                        }else {
                            currentNode.children(".scontainer-node-render-container").first().addClass("scontainer-node-highlight");
                        }
                    }
                };
            }
        },

        _setOption: function (key, value) {
            mySelf._super(key, value);

            if (key === "source") {
                mySelf._getChildrenNodes(0, mySelf.element, mySelf._getChildrenNodesCompleted);
            }

            if (key === "expandPaths") {
                mySelf._expandNodesInExpandPaths(mySelf.options.expandPaths);
            }
        },

        destroy: function () {
            mySelf.element.html("");
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);