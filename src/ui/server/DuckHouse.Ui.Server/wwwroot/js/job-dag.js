// Interactive DAG visualization for job task graphs.
// Requires cytoscape.min.js, dagre.min.js, and cytoscape-dagre.js loaded before this file.

(function () {
    var _instances = {};

    function getTypeLabel(taskType) {
        if (taskType === 'SqlQuery') return 'SQL Query';
        if (taskType === 'SubJob') return 'Sub-Job';
        return taskType;
    }

    function getConditionLabel(condition) {
        if (condition === 'OnSuccess') return 'success';
        if (condition === 'OnFailure') return 'failure';
        if (condition === 'OnCompletion') return 'completion';
        if (condition === 'OnSkip') return 'skip';
        return condition;
    }

    function buildNodeLabel(name, taskType, itemName) {
        var typeStr = getTypeLabel(taskType);
        var maxLen = 28;
        var item = itemName || '';
        if (item.length > maxLen) {
            item = '\u2026' + item.slice(-(maxLen - 1));
        }
        return item ? name + '\n' + typeStr + ' \u00b7 ' + item : name + '\n' + typeStr;
    }

    function getStyle(isDark) {
        var nodeBg    = isDark ? '#1a1a1a' : '#ffffff';
        var border    = isDark ? '#434343' : '#d9d9d9';
        var text      = isDark ? 'rgba(255,255,255,0.85)' : 'rgba(0,0,0,0.85)';
        var subText   = isDark ? 'rgba(255,255,255,0.45)' : 'rgba(0,0,0,0.55)';
        var edgeLine  = isDark ? '#595959' : '#bfbfbf';
        var hoverBg   = isDark ? '#2a2a2a' : '#f0f7ff';
        var labelBg   = isDark ? '#1a1a1a' : '#ffffff';

        return [
            {
                selector: 'node',
                style: {
                    'shape': 'round-rectangle',
                    'width': 200,
                    'height': 62,
                    'background-color': nodeBg,
                    'border-width': 2,
                    'border-color': border,
                    'label': 'data(label)',
                    'text-valign': 'center',
                    'text-halign': 'center',
                    'font-size': 12,
                    'color': text,
                    'text-wrap': 'wrap',
                    'text-max-width': 180,
                    'line-height': 1.5,
                    'cursor': 'pointer',
                },
            },
            {
                selector: 'node[taskType="Notebook"]',
                style: { 'border-color': '#1890ff' },
            },
            {
                selector: 'node[taskType="SqlQuery"]',
                style: { 'border-color': '#722ed1' },
            },
            {
                selector: 'node[taskType="SubJob"]',
                style: { 'border-color': '#fa8c16' },
            },
            {
                selector: 'node:active',
                style: { 'overlay-opacity': 0.08 },
            },
            {
                selector: 'node:hover',
                style: { 'background-color': hoverBg },
            },
            {
                selector: 'edge',
                style: {
                    'width': 1.5,
                    'line-color': edgeLine,
                    'target-arrow-color': edgeLine,
                    'target-arrow-shape': 'triangle',
                    'curve-style': 'bezier',
                    'label': 'data(label)',
                    'font-size': 10,
                    'color': subText,
                    'text-background-color': labelBg,
                    'text-background-opacity': 1,
                    'text-background-padding': '2px',
                    'text-rotation': 'autorotate',
                },
            },
            {
                selector: 'edge[condition="OnSuccess"]',
                style: {
                    'line-color': '#52c41a',
                    'target-arrow-color': '#52c41a',
                },
            },
            {
                selector: 'edge[condition="OnFailure"]',
                style: {
                    'line-color': '#ff4d4f',
                    'target-arrow-color': '#ff4d4f',
                },
            },
            {
                selector: 'edge[condition="OnCompletion"]',
                style: {
                    'line-color': edgeLine,
                    'target-arrow-color': edgeLine,
                },
            },
            {
                selector: 'edge[condition="OnSkip"]',
                style: {
                    'line-color': '#fa8c16',
                    'target-arrow-color': '#fa8c16',
                },
            },
        ];
    }

    window.dagGraph = {
        init: function (containerId, dotNetRef, isDark) {
            var container = document.getElementById(containerId);
            if (!container) return;

            if (_instances[containerId]) {
                try { _instances[containerId].cy.destroy(); } catch (_) { }
            }

            var cy = cytoscape({
                container: container,
                wheelSensitivity: 0.3,
                minZoom: 0.1,
                maxZoom: 4,
                userZoomingEnabled: true,
                userPanningEnabled: true,
                boxSelectionEnabled: false,
                style: getStyle(isDark),
            });

            cy.on('tap', 'node', function (evt) {
                var name = evt.target.data('name');
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnDagNodeClicked', name).catch(function () { });
                }
            });

            _instances[containerId] = { cy: cy, dotNetRef: dotNetRef };
        },

        update: function (containerId, nodes, edges) {
            var inst = _instances[containerId];
            if (!inst) return;
            var cy = inst.cy;

            // Build elements with labels
            var nodeElements = nodes.map(function (n) {
                return {
                    data: {
                        id: n.name,
                        name: n.name,
                        taskType: n.taskType,
                        label: buildNodeLabel(n.name, n.taskType, n.itemName),
                    },
                };
            });

            var edgeElements = edges.map(function (e) {
                return {
                    data: {
                        id: e.source + '\u2192' + e.target + '\u2192' + e.condition,
                        source: e.source,
                        target: e.target,
                        condition: e.condition,
                        label: getConditionLabel(e.condition),
                    },
                };
            });

            cy.batch(function () {
                cy.elements().remove();
                cy.add(nodeElements.concat(edgeElements));
            });

            if (cy.nodes().length > 0) {
                cy.layout({
                    name: 'dagre',
                    rankDir: 'LR',
                    nodeSep: 40,
                    rankSep: 80,
                    padding: 24,
                    fit: true,
                }).run();
            }
        },

        setTheme: function (containerId, isDark) {
            var inst = _instances[containerId];
            if (!inst) return;
            inst.cy.style(getStyle(isDark)).update();
        },

        fit: function (containerId) {
            var inst = _instances[containerId];
            if (!inst) return;
            inst.cy.fit(undefined, 30);
        },

        dispose: function (containerId) {
            var inst = _instances[containerId];
            if (!inst) return;
            try { inst.cy.destroy(); } catch (_) { }
            delete _instances[containerId];
        },

        isDark: function () {
            return document.documentElement.classList.contains('ant-dark');
        },
    };
}());
