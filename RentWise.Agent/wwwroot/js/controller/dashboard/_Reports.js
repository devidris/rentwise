
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
        labels: ['Number of Sales', 'Total number of orders'],
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