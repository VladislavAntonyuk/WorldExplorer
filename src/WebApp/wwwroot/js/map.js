let map;
let userMarker;
window.leafletInterop = {
	initMap: function(dotnetRef, options) {
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
					if (location.lat === e.latlng.lat && location.lng === e.latlng.lng) {
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
				dotnetRef.invokeMethodAsync("UpdatePosition", { latitude: e.latlng.lat, longitude: e.latlng.lng });
			});
		map.on("locationerror",
			(e) => {
				dotnetRef.invokeMethodAsync("UpdatePositionError", e.message);
			});
	},
	addMarker: function(dotnetRef, options) {
		const marker = createMarker(options);
		marker.on("click",
			(e) => {
				dotnetRef.invokeMethodAsync("OpenDetails",
					options.title,
					{ latitude: e.latlng.lat, longitude: e.latlng.lng });
			});
	},
	destroyMap: function() {
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
	return L.marker([options.location.latitude, options.location.longitude], { icon: icon, title: options.title })
		.addTo(map);
}