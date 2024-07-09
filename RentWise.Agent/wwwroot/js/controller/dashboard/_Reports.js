const jsonOrders = $('.orders').text()
$('.orders').remove()
const orders = JSON.parse(jsonOrders)
const orderStatus = [
    "Pending",
    "Accepted",
    "Rejected",
    "Paid",
    "Cancelled",
    "Expired",
    "Waiting to pay with cash"]


function onToggleReport(contentNo) {
    document.querySelectorAll(".report").forEach((content) => {
        content.classList.add("display-none");

    })
    document.querySelectorAll(".report-tab").forEach((content) => {
        content.classList.remove("report-active");

    })

    document.querySelector(".report-" + contentNo).classList.remove("display-none");
    document.querySelector(".report-tab-" + contentNo).classList.add("report-active");
}

onToggleReport(1);
document.addEventListener('DOMContentLoaded', function () {
    var ctx = document.getElementById('overview-report').getContext('2d');

    var data = {
        labels: ['Total Sales', 'Total number of Reservations'],
        datasets: [{
            label: 'Count',
            data: [totalEarnings, totalOrders],
            backgroundColor: [
                'rgba(0, 128, 128, 1)',
                'rgba(75, 0, 130, 1)'
            ],
            borderColor: [
                'rgba(0, 128, 128, 1)',
                'rgba(75, 0, 130, 1)'
            ],
            borderWidth: 1,
            barThickness: 20 
        }]
    };

    var options = {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    };

    var myChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options
    });
});

function getReportData() {
    let startDate = document.querySelector('.spd-from').value;
    let endDate = document.querySelector('.spd-to').value;
    if (startDate === '' || endDate === '') {
        calculateSales(orders);
    } else {
        const startDate = new Date(document.querySelector('.spd-from').value);
        const endDate = new Date(document.querySelector('.spd-to').value);
        const filteredOrdersByDates = orders.filter(order => {
            const orderDate = new Date(order.CreatedAt);
            return orderDate >= startDate && orderDate < endDate;
        });
        calculateSales(filteredOrdersByDates);
    }
}
function calculateSales(orders) {
    const filteredOrders = orders.filter(order => order.LkpOderStaus == 4)
    const pendingOrders = orders.filter(order => order.LkpStatus == 1 || order.LkpStatus == 2 || order.LkpStatus == 7)
    const totalEarnings = filteredOrders.reduce((total, order) => total + order.TotalAmount, 0)
    const averageEarnings = totalEarnings == 0 ? 0 : totalEarnings / filteredOrders.length
    const currency = "₵"
    $('.sitp').text(currency + totalEarnings)
    $('.ads').text(currency + averageEarnings)
    $('.opg').text(pendingOrders.length)
    $('.opd').text(filteredOrders.length)

    const dailyTotals = {};

    orders.forEach((order) => {
        const createdAtDate = order.CreatedAt.split("T")[0]; // Extract date part
        dailyTotals[createdAtDate] = (dailyTotals[createdAtDate] || 0) + order.TotalAmount;
    });

    const dailyTotalsArray = Object.entries(dailyTotals).map(([date, total]) => ({
        date,
        total,
    }));

    displayOverviewReport2(dailyTotalsArray);
}

getReportData();
function displayOverviewReport2(dailyTotalsArray) {
    var ctx = document.getElementById('overview-report-2').getContext('2d');

    // Check if a chart already exists
    var existingChart = Chart.getChart(ctx);
    if (existingChart) {
        existingChart.destroy(); // Destroy the existing chart
    }

    const dates = dailyTotalsArray.map(entry => entry.date);
    const totals = dailyTotalsArray.map(entry => entry.total);

    const myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: dates,
            datasets: [{
                label: 'Daily Totals',
                data: totals,
                backgroundColor: 'rgba(75, 192, 192, 1)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function getReportDataTable(type = "selling") {


    const className = type == "earning" ? "te" : "ts"
    let startDate = document.querySelector('.' + className +'-from').value;
    let endDate = document.querySelector('.' + className + '-to').value;
    if (startDate == '' || endDate == '') {
        let tableData = ""
        let resultArray = groupOrders(orders)
        if (type == "earning") {
            resultArray = resultArray.sort((a, b) => b.TotalAmount - a.TotalAmount)
        } else {
            resultArray = resultArray.sort((a, b) => b.Quantity - a.Quantity)
        }
        resultArray.forEach((item,index) => {
            tableData += `
             <tr>
                    <th scope="row">${index+1}</th>
                        <td>${item.Quantity}</td>
                        <td>${item.ProductName}</td>
                        <td>${item.TotalAmount}</td>
                </tr>
            `;
        })
        $('.' + className + '-tbody').html(tableData)
    
    } else {
        const startDate = new Date(document.querySelector('.spd-from').value);
        const endDate = new Date(document.querySelector('.spd-to').value);
        const filteredOrdersByDates = orders.filter(order => {
            const orderDate = new Date(order.CreatedAt);
            return orderDate >= startDate && orderDate < endDate;
        });
        let tableData = ""
        let resultArray = groupOrders(filteredOrdersByDates)
        if (type == "earning") {
            resultArray = resultArray.sort((a, b) => b.TotalAmount - a.TotalAmount)
        } else {
            resultArray = resultArray.sort((a, b) => b.Quantity - a.Quantity)
        }
        resultArray.forEach((item, index) => {
            tableData += `
             <tr>
                    <th scope="row">${index + 1}</th>
                        <td>${item.Quantity}</td>
                        <td>${item.ProductName}</td>
                        <td>${item.TotalAmount}</td>
                </tr>
            `;
        })
        $('.' + className + '-tbody').html(tableData)

    }
}

function groupOrders(orders) {

    const groupedOrders = {};

    orders.forEach(order => {
        const productId = order.ProductId;

        if (!groupedOrders[productId]) {
            groupedOrders[productId] = {
                Quantity: 0,
                ProductName: order.Product.Name,
                TotalAmount: 0,
            };
        }

        groupedOrders[productId].Quantity += order.ProductQuantity;
        groupedOrders[productId].TotalAmount += order.TotalAmount;
    });

    return  Object.entries(groupedOrders).map(([productId, values]) => ({
        ProductId: productId,
        Quantity: values.Quantity,
        ProductName: values.ProductName,
        TotalAmount: values.TotalAmount,
    }));
}
getReportDataTable()
getReportDataTable('earning')