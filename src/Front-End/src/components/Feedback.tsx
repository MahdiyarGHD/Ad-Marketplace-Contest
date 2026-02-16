import { memo, useState } from "react";
import MainButton from "./MainButton";
import { Textarea } from "./Textarea";

function Feedback({ onSubmit }: { onSubmit: (feedback: string) => void }) {
	const [feedback, setFeedback] = useState("");

	return (
		<div className="Feedback">
			<div className="Items">
				<div className="Item">
					<div className="body">
						<Textarea
							style={{ minHeight: 64 }}
							placeholder="Feedback"
							value={feedback}
							onChange={(e) => setFeedback(e.target.value)}
						/>
					</div>
				</div>
			</div>
			<MainButton text="Submit" onClick={() => onSubmit(feedback)} />
		</div>
	);
}

export default memo(Feedback);
