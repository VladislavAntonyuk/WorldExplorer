let map;
let userMarker;
window.leafletInterop = {
	initMap: function (dotnetRef, options) {
		map = L.map("map",
			{
				zoom: options.zoom
			});
		L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png",
			{
				attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
			}).addTo(map);
		map.locate({
			watch: true,
			setView: false,
			enableHighAccuracy: true,
			timeout: Infinity
		});
		map.on("locationfound",
			(e) => {
				if (userMarker) {
					const location = userMarker.getLatLng();
					if (isNearbyLocation(location.lat, e.latlng, 100)) {
						return;
					}

					userMarker.removeFrom(map);
				}

				userMarker = createMarker({
					icon: "/assets/user-location-pin.png",
					location: {
						latitude: e.latlng.lat,
						longitude: e.latlng.lng
					},
					title: "My location"
				});
				map.setView(e.latlng, options.zoom);
				dotnetRef.invokeMethodAsync("UpdatePosition", {latitude: e.latlng.lat, longitude: e.latlng.lng});
			});
		map.on("locationerror",
			(e) => {
				dotnetRef.invokeMethodAsync("UpdatePositionError", e.message);
			});
	},
	addMarker: function (dotnetRef, id, options) {
		const marker = createMarker(options);
		marker.on("click",
			(e) => {
				dotnetRef.invokeMethodAsync("OpenDetails", id,	options.title);
			});
	},
	destroyMap: function () {
		if (userMarker) {
			userMarker.remove();
			userMarker = undefined;
		}

		if (map) {
			map.stopLocate();
			map.remove();
		}
	}
};

function createMarker(options) {
	const icon = L.icon({
		iconUrl: options.icon
	});
	return L.marker([options.location.latitude, options.location.longitude], {icon: icon, title: options.title})
		.addTo(map);
}

function isNearbyLocation(location1, location2, distance) {
	const metersPerDegree = 111139; // Approximate for both latitude and longitude
	const latLongDifferenceEquivalentToM = distance / metersPerDegree;

	const latDifference = Math.abs(location1.lat - location2.lat);
	const longDifference = Math.abs(location1.lng - location2.lng);

	return (
		latDifference <= latLongDifferenceEquivalentToM &&
		longDifference <= latLongDifferenceEquivalentToM
	);
}