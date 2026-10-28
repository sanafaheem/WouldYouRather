export default function Home() {
	return (
		<main className="d-flex flex-grow-1 align-items-center justify-content-center py-5">
			<div className="container">
				<div className="card shadow-sm p-4 p-md-5 text-center">
					<p className="text-uppercase text-primary fw-semibold small mb-2">
						Today's Question
					</p>
					<h1 className="h3 mb-4">
						Would you rather have the ability to fly or be invisible?
					</h1>
					<div className="row g-3">
						<div className="col-12 col-sm-6">
							<button type="button" className="btn btn-outline-primary w-100 py-3">
								Fly through the sky
							</button>
						</div>
						<div className="col-12 col-sm-6">
							<button type="button" className="btn btn-outline-primary w-100 py-3">
								Turn invisible
							</button>
						</div>
					</div>
				</div>
			</div>
		</main>
	);
}
