const totalOrders = $(".order-count").val()
const totalEarnings = $(".earnings").val()
document.addEventListener('DOMContentLoaded', function () {
    var ctx = document.getElementById('myChart').getContext('2d');

    var data = {
        labels: ['Total Sales', 'Total number of orders'],
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
