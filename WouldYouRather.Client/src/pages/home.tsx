import {useEffect, useState} from "react";
import { getTodayQuestion, submitVote } from "../services/questionService";
import type { Question, VoteResponse } from "../types/question";
export default function Home() {

	const [question, setQuestion] = useState<Question | null>(null);
	const [voteResponse, setVoteResponse] = useState<VoteResponse | null>(null);
	const [hasVoted, setHasVoted] = useState(false);
	const [submittedVote, setSubmittedVote] = useState<"A" | "B" | null>(null);
	useEffect(() => {
		let isMounted = false; // Flag to track if the component is still mounted
		getTodayQuestion().then((question) => {
			if (!isMounted) {
				// Update the state or perform any necessary actions with the question data
				setQuestion(question);
			}
		}).catch((error) => {
			if (!isMounted) {
			console.error("Error fetching today's question:", error);
			}
		});
		return () => {
			isMounted = true; // Set the flag to true when the component is unmounted
		};
	}, []);

	function handleVote(questionID: string, option: 'A' | 'B') {
		setHasVoted(true);
		setSubmittedVote(option);
		submitVote({ questionId: questionID, option: option }).then((response) => {
			setVoteResponse(response);
			// Handle successful vote submission if needed
		}).catch((error) => {
			setHasVoted(false); // Reset the voting state if there's an error
			setSubmittedVote(null); // Reset the submitted vote if there's an error
			console.error("Error submitting vote:", error);
		});
	}

	return (
		<main className="flex-grow-1 pt-4 pt-md-5 pb-3">
			<div className="container">
				<div className="row justify-content-center">
					<div className="col-12 col-md-8 col-lg-8">
						<div className="card shadow-sm p-4 p-md-5 text-center">
							<p className="text-uppercase text-primary fw-semibold small mb-2">
								Today's Question
							</p>
							<h1 className="h3 mb-4">
								 {question?.questionDescription || "Loading..."}
							</h1>
							{(voteResponse && hasVoted) ? (
								<div>
									 <div className="d-flex justify-content-between mb-1">
										<span>{question?.optionA} {submittedVote === "A" && <span className="badge bg-primary bg-opacity-10 text-primary ms-2">Your Vote</span>}</span> 
									  	<span> {voteResponse.percentA}%</span></div>
										<div className="progress mb-3">
											<div className="progress-bar bg-primary" style={{ width: `${voteResponse.percentA}%` }}>
											{voteResponse.percentA}%
										</div>
									</div>
									<div className="d-flex justify-content-between mb-1">
										<span>{question?.optionB} {submittedVote === "B" && <span className="badge bg-primary bg-opacity-10 text-primary ms-2">Your Vote</span>}</span> 
										<span> {voteResponse.percentB}%</span></div>
									<div className="progress mb-3">
										<div className="progress-bar bg-primary" style={{ width: `${voteResponse.percentB}%` }}>
											{voteResponse.percentB}%
										</div>
									</div>
									<p className="text-muted small fst-italic">{voteResponse.wittyComment}</p>
								</div>
							) : (<div className="row g-3">
								<div className="col-12 col-sm-6">
									<button type="button" disabled={hasVoted || !question} onClick={() => handleVote(question?.id || "", "A")} className="btn btn-outline-primary w-100 py-3">
										{question?.optionA || "Loading..."}
									</button>
								</div>
								<div className="col-12 col-sm-6">
									<button type="button" disabled={hasVoted || !question} onClick={() => handleVote(question?.id || "", "B")} className="btn btn-outline-primary w-100 py-3">
										{question?.optionB || "Loading..."}
									</button>
								</div>
							</div>)}
							
						</div>
					</div>
				</div>
			</div>
		</main>
	);
}


